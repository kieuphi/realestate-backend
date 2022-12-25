using System;
using System.Collections.ObjectModel;

namespace General.Domain.Enumerations
{
    public struct ImageCategories
    {
        public const string property = nameof(property);
        public const string news = nameof(news);
        public const string user = nameof(user);
        public const string banner = nameof(banner);
        public const string site = nameof(site);
        public const string project = nameof(project);

        public static ReadOnlyCollection<string> GetAllowAttachmentTypes(string imageCategory)
        {
            return imageCategory switch
            {
                property => new ReadOnlyCollection<string>(new string[] { ".png", ".jpg", ".jpeg" }),
                news => new ReadOnlyCollection<string>(new string[] { ".png", ".jpg", ".jpeg" }),
                user => new ReadOnlyCollection<string>(new string[] { ".png", ".jpg", ".jpeg" }),
                banner => new ReadOnlyCollection<string>(new string[] { ".png", ".jpg", ".jpeg" }),
                site => new ReadOnlyCollection<string>(new string[] { ".png", ".jpg", ".jpeg" }),
                project => new ReadOnlyCollection<string>(new string[] { ".png", ".jpg", ".jpeg" }),
                _ => throw new NotSupportedException()
            };
        }

        public static string GetFolderToUploadByType(string imageCategory)
        {
            return imageCategory switch
            {
                property => "property",
                news => "news",
                user => "user",
                banner => "banner",
                site => "site",
                project => "project",
                _ => throw new NotSupportedException()
            };
        }

        public static long GetFileSizeLimitByType(string imageCategory)
        {
            return imageCategory switch
            {
                property => 2097152000,
                news => 20971520000,
                user => 2097152000,
                banner => 2097152000,
                site => 2097152000,
                project => 2097152000,
                _ => throw new NotSupportedException()
            };
        }
    }
}
