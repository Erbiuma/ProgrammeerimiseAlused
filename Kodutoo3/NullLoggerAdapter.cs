namespace Kodutoo3;

internal class NullLoggerAdapter : BaseLogger
{
    private readonly NullLogger logger = new NullLogger();

    public override void Info(string message)
    {
        logger.WriteInfo(message);
    }

    public override void Debug(string message)
    {
        logger.WriteDebug(message);
    }

    public override void Warning(string message)
    {
        logger.WriteWarning(message);
    }

    public override void Error(string message)
    {
        logger.WriteError(message);
    }
}