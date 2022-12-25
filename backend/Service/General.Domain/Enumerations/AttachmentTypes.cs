using System;
using System.Collections.ObjectModel;

namespace General.Domain.Enumerations
{
    public struct AttachmentTypes
    {
        public const string photo = nameof(photo);
        public const string video = nameof(video);
        public const string excel = nameof(excel);
        public const string audio = nameof(audio);

        public static ReadOnlyCollection<string> GetAllowAttachmentTypes(string attachmentType)
        {
            return attachmentType switch
            {
                photo => new ReadOnlyCollection<string>(new string[] { ".png", ".PNG", ".jpg", ".jpeg",".svg" }),
                video => new ReadOnlyCollection<string>(new string[] { ".mp4", ".avi" }),
                excel => new ReadOnlyCollection<string>(new string[] { ".xls", ".xlsx" }),
                audio => new ReadOnlyCollection<string>(new string[] { ".mp3", ".wav" }),
                _ => throw new NotSupportedException()
            };
        }

        public static string GetFolderToUploadByType(string attachmentTypes)
        {
            return attachmentTypes switch
            {
                photo => "photos",
                video => "videos",
                excel => "files",
                audio => "audios",
                _ => throw new NotSupportedException()
            };
        }

        public static long GetFileSizeLimitByType(string attachmentTypes)
        {
            return attachmentTypes switch
            {
                photo => 2097152000,
                video => 20971520000,
                excel => 2097152000,
                audio => 20971520000,
                _ => throw new NotSupportedException()
            };
        }
    }
}
