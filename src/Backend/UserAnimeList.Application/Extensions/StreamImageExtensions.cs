using FileTypeChecker;

namespace UserAnimeList.Application.Extensions;

public static class StreamImageExtensions
{
    private static readonly List<string> AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];
    public static bool Validate(this Stream? stream)
    {
        if (stream is null || stream.Length == 0)
            return false;

        var originalPosition = stream.CanSeek ? stream.Position : 0;
        try
        {
            var fileType = FileTypeValidator.GetFileType(stream);

            return fileType != null && AllowedExtensions.Contains("." + fileType.Extension);
        }
        finally
        {
            if (stream.CanSeek)
                stream.Position = originalPosition;
        }
    }

    public static string GetImageExtension(this Stream? stream)
    {
        if (stream is null || stream.Length == 0)
            return string.Empty;

        var originalPosition = stream.CanSeek ? stream.Position : 0;
        
        try
        {
            var fileType = FileTypeValidator.GetFileType(stream);

            return fileType != null? fileType.Extension : string.Empty;
        }
        finally
        {
            if (stream.CanSeek)
                stream.Position = originalPosition;
        }
    }

}