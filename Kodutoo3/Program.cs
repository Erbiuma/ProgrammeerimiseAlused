using Kodutoo3;

BaseLogger[] loggers = { new ConsoleLogger(), new DebugLogger(), new NullLoggerAdapter() };

foreach (BaseLogger logger in loggers)
{
    Console.WriteLine(logger.ToString());

    logger.Info("Programm käivitus.");
    logger.Debug("Loeti 20 uudist.");
    logger.Warning("Ühel uudisel puudub pealkiri.");
    logger.Error("Faili avamine ebaõnnestus.");
}