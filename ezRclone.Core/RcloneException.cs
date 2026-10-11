namespace ezRclone.Core;

public sealed class RcloneException(string message, Exception? innerException = null)
    : Exception(message, innerException);
