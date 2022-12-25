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
using System.ComponentModel.DataAnnotations;

namespace General.Application.Config.Commands
{
    public class SaveConfigCommand : IRequest<Result>
    {
        public CreateConfigModel Model { set; get; }
    }

    public class SaveConfigCommandHandler : IRequestHandler<SaveConfigCommand, Result>
    {
        private readonly ILogger<SaveConfigCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IApplicationDbContext _context;

        public SaveConfigCommandHandler(
            ILogger<SaveConfigCommandHandler> logger,
            IMapper mapper,
            IApplicationDbContext context
        ) 
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Result> Handle(SaveConfigCommand request, CancellationToken cancellationToken)
        {
            ConfigEntity configEntity = await _context.Config.FirstOrDefaultAsync();
            // validate
            var emailCheck = new EmailAddressAttribute();
            string[] arrEmail = request.Model.ReceiveEmailContactUs.Split(';');
            List<string> listError = new List<string>();
            string strReceiveEmailContactUs = "";

            for (int i = 0; i < arrEmail.Length; i++)
            {
                if(arrEmail[i].Trim() == "")
                {
                    continue;
                }
                if(emailCheck.IsValid(arrEmail[i]) == false)
                {
                    listError.Add($"The email {arrEmail[i]} is invalid format.");
                }
                strReceiveEmailContactUs += arrEmail[i] + ";";
            }

            string strReceiveEmailBookShowing = "";
            arrEmail = request.Model.ReceiveEmailBookShowing.Split(';');
            for (int i = 0; i < arrEmail.Length; i++)
            {
                if (arrEmail[i].Trim() == "")
                {
                    continue;
                }
                if (emailCheck.IsValid(arrEmail[i]) == false)
                {
                    listError.Add($"The email {arrEmail[i]} is invalid format.");
                }
                strReceiveEmailBookShowing += arrEmail[i] + ";";
            }

            if (listError.Count > 0)
            {
                return Result.Failure(listError);
            }

            if (strReceiveEmailContactUs.Length > 0)
            {
                strReceiveEmailContactUs = strReceiveEmailContactUs.Substring(0, strReceiveEmailContactUs.Length - 1);
            }
            if (strReceiveEmailBookShowing.Length > 0)
            {
                strReceiveEmailBookShowing = strReceiveEmailBookShowing.Substring(0, strReceiveEmailBookShowing.Length - 1);
            }

            if (configEntity == null)
            {
                configEntity = new ConfigEntity
                {
                    ReceiveEmailContactUs = request.Model.ReceiveEmailContactUs,
                    ReceiveEmailBookShowing = request.Model.ReceiveEmailBookShowing
                };
                _context.Config.Add(configEntity);
            }

            configEntity.ReceiveEmailContactUs = strReceiveEmailContactUs;
            configEntity.ReceiveEmailBookShowing = strReceiveEmailBookShowing;
            if (string.IsNullOrEmpty(request.Model.Host) == false)
            {
                configEntity.Host = request.Model.Host;
                configEntity.Port = request.Model.Port;
                configEntity.UserName = request.Model.UserName;
                configEntity.Password = request.Model.Password;
                configEntity.DisplayName = request.Model.DisplayName;
            }
            await _context.SaveChangesAsync();

            return Result.Success();
        }

        
    }
}
