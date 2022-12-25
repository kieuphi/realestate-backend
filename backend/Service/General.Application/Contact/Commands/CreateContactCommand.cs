using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Common.Shared.Models;
using General.Domain.Models;
using System.Threading.Tasks;
using System.Threading;
using AutoMapper;
using General.Application.Interfaces;
using General.Application.Common.Interfaces;
using General.Domain.Entities;
using System.Linq;
using General.Application.Contact.Queries;
using Microsoft.EntityFrameworkCore;
using General.Application.Email.Commands;
using Microsoft.Extensions.Logging;
using General.Application.Property.Queries;
using General.Domain.Models.PropertyElementModels;
using General.Application.Project.Queries;
using General.Domain.Models.ProjectElementModels;
using General.Domain.Enums;

namespace General.Application.Contact.Commands
{
    public class CreateContactCommand : IRequest<Result>
    {
        public CreateContactModel Model { set; get; }
        public string Domain { get; set; }
    }

    public class CreateContactCommandHandler : IRequestHandler<CreateContactCommand, Result>
    {
        private readonly ILogger<CreateContactCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IApplicationDbContext _context;
        private readonly IAsyncRepository<ContactEntity> _repository;
        private readonly IMediator _mediator;
        private readonly IIdentityService _identityService;

        public CreateContactCommandHandler(
            ILogger<CreateContactCommandHandler> logger,
            IMapper mapper,
            IApplicationDbContext context,
            IAsyncRepository<ContactEntity> repository,
            IMediator mediator,
            IIdentityService identityService
        ) 
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
        }

        public async Task<Result> Handle(CreateContactCommand request, CancellationToken cancellationToken)
        {
            var model = request.Model;
            var newId = Guid.NewGuid();

            var contact = await _context.Contact.Where(x => x.Id == newId).FirstOrDefaultAsync();
            if (contact != null)
            {
                return Result.Failure($"The specified Contact is invalid: {newId}");
            }
            if (string.IsNullOrEmpty(model.Email))
            {
                return Result.Failure("The email is required");
            }
            if (string.IsNullOrEmpty(model.Name))
            {
                return Result.Failure("The your name is required");
            }

            PropertyModel propertyInfo = null;
            ProjectModel projectInfo = null;
            if (model.ContactType == Domain.Enums.ContactType.Email || model.ContactType == Domain.Enums.ContactType.Book)
            {
                if (string.IsNullOrEmpty(model.PropertyId) == false)
                {
                    propertyInfo = await _mediator.Send(new GetPropertyByIdQuery() { Id = Guid.Parse(model.PropertyId), IsAdmin = false });
                    if (propertyInfo == null)
                    {
                        return Result.Failure("The property id is invalid");
                    }
                }
                else if (string.IsNullOrEmpty(model.ProjectId) == false)
                {
                    projectInfo = await _mediator.Send(new GetProjectByIdQuery() { Id = Guid.Parse(model.ProjectId) });
                    if (projectInfo == null)
                    {
                        return Result.Failure("The project id is invalid");
                    }
                }
                else
                {
                    return Result.Failure("The PropertyId field is required");
                }
            }

            ContactEntity entity = new ContactEntity()
            {
                Id = newId,
                Subject = model.Subject,
                Name = model.Name,
                Email = model.Email,
                Phone = model.Phone,
                Message = model.Message,
                ContactType = model.ContactType
            };

            await _repository.AddAsync(entity);
            await _context.SaveChangesAsync(new CancellationToken());
            //send mail
            ConfigEntity configInfo = await _context.Config.FirstOrDefaultAsync();
            try
            {
                await SendMailForUser(model.Email, model.Name, model.Message,request.Domain, entity.ContactType);
                if (model.ContactType == Domain.Enums.ContactType.Contact || model.ContactType == Domain.Enums.ContactType.Question)
                {
                    // send mail admin 
                    await SendMailForAdmin(configInfo, entity.Subject, entity.Name, entity.Message, entity.Email, entity.Phone, request.Domain, entity.ContactType);
                }
            }
            catch(Exception ex)
            {
            }
            if(model.ContactType == Domain.Enums.ContactType.Email || model.ContactType == Domain.Enums.ContactType.Book)
            {
                List<string> listEmailAdmin = new List<string>();
                if (configInfo != null)
                {
                    listEmailAdmin = configInfo.ReceiveEmailBookShowing.Split(";").ToList();
                    listEmailAdmin = listEmailAdmin.Where(x => string.IsNullOrEmpty(x) == false).ToList();
                }

                string subject = "";
                if (entity.ContactType == ContactType.Book)
                {
                    subject = "BOOK SHOWING REQUEST";
                }
                else if (entity.ContactType == ContactType.Email)
                {
                    subject = entity.Subject;
                }

                if (propertyInfo != null)
                {
                    if (propertyInfo.PropertySellers.Count > 0)
                    {
                        for (int i = 0; i < propertyInfo.PropertySellers.Count; i++)
                        {
                            try
                            {
                                await SendMailForSeller(propertyInfo, propertyInfo.PropertySellers[i], listEmailAdmin, subject, entity.Email, entity.Name, entity.Phone, entity.Message, entity.ContactType, request.Domain);
                                listEmailAdmin = new List<string>();
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }
                }
                else if(projectInfo != null)
                {
                    if (projectInfo.ProjectSellers.Count > 0)
                    {
                        for (int i = 0; i < projectInfo.ProjectSellers.Count; i++)
                        {
                            try
                            {
                                await SendMailProjectForSeller(projectInfo, projectInfo.ProjectSellers[i], listEmailAdmin, subject, entity.Email, entity.Name, entity.Phone, entity.Message, request.Domain);
                                listEmailAdmin = new List<string>();
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }
                }
            }

            var result = await _mediator.Send(new GetContactByIdQuery() { Id = entity.Id });
            return Result.Success(result);
        }

        private async Task SendMailForUser(string email, string fullName, string message,string domain, ContactType? contactType)
        {
            await _mediator.Send(new SendMailContactForUserCommand
            {
                RequestModel = new ContactUsEmailModel
                {
                    Email = email,
                    CustomerName = fullName,
                    CustomerMessage = message,
                },
                Domain = domain,
                ContactType = contactType
            });
        }

        private async Task SendMailForAdmin(ConfigEntity configInfo, string subject,string customerName, string customerMessage,
            string customerEmail,string customerPhone, string domain, ContactType contactType)
        {
            if (configInfo == null)
            {
                return;
            }

            string[] arrEmail = configInfo.ReceiveEmailContactUs.Split(";");
            await _mediator.Send(new SendMailContactForAdminCommand
            {
                RequestModel = new ContactUsEmailModel
                {
                    CustomerName = customerName,
                    CustomerMessage = customerMessage,
                    CustomerEmail = customerEmail,
                    CustomerPhone = customerPhone,
                    Subject = subject
                },
                ListEmail= arrEmail.ToList(),
                Domain = domain,
                ContactType = contactType
            });
        }

        private async Task SendMailForSeller(PropertyModel propertyInfo, PropertySellerModel profileInfo,List<string> listEmailCC, 
            string subject,string customerEmail, string customerName, string customerPhone, string customerMessage, Domain.Enums.ContactType contactType, string domain)
        {
            await _mediator.Send(new SendMailContactForSellerCommand
            {
                RequestModel = new BookShowingEmailModel
                {
                    SellerEmail = profileInfo.Email,
                    ListEmailCC = listEmailCC,
                    SellerName = profileInfo.LastName + " " + profileInfo.FirstName,
                    CustomerEmail = customerEmail,
                    CustomerName = customerName,
                    CustomerPhone = customerPhone,
                    CustomerMessage = customerMessage,
                    ContactType = contactType.ToString(),
                    PropertyNumber = propertyInfo.PropertyNumber,
                    PropertyTitle = propertyInfo.Title,
                    TransactionType = propertyInfo.TransactionTypeEn,
                    PropertyType = propertyInfo.PropertyTypeEn,
                    Subject = subject
                },
                Domain = domain
            });
        }

        private async Task SendMailProjectForSeller(ProjectModel projectInfo, ProjectSellerModel profileInfo, List<string> listEmailCC, 
            string subject,string customerEmail,string customerName, string customerPhone, string customerMessage, 
            string domain)
        {
            await _mediator.Send(new SendMailContactForSellerCommand
            {
                RequestModel = new BookShowingEmailModel
                {
                    SellerEmail = profileInfo.Email,
                    ListEmailCC = listEmailCC,
                    SellerName = profileInfo.LastName + " " + profileInfo.FirstName,
                    CustomerEmail = customerEmail,
                    CustomerName = customerName,
                    CustomerPhone = customerPhone,
                    CustomerMessage = customerMessage,
                    ProjectName = projectInfo.ProjectEn,
                    Province= projectInfo.ProvinceName,
                    District= projectInfo.DistrictName,
                    Subject = subject
                },
                Domain = domain
            });
        }
    }
}
