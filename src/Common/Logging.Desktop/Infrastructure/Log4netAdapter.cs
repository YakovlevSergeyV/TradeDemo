
namespace Logging.Desktop.Infrastructure
{
    using System;
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using GlobalSpace.Common.Guardly;
    using log4net;
    using log4net.Appender;
    using log4net.Config;
    using log4net.Repository.Hierarchy;
    using Logging.Desktop.Abstract;

    internal sealed class Log4netAdapter : ILog4netAdapter
    {
        private const string ConstLog4netAppenderName = "Log4netAppenderName";
        private const string XmlElementFile = "file";
        private const string XmlAttributeValue = "value";
        private const string ErrorCalculationLogFilename = "Ошибка вычисления имени файла лога";
        private const string Log4netAppenderNameError = "Ошибка установки имени log4net appender";
        private string log4netAppenderName;

        [ExcludeFromCodeCoverage]
        public void SetFilePathFromCommandLine(Log4netSettings log4netSettings)
        {
            Guard.Argument(() => log4netSettings, Is.NotNull);
            Guard.Argument(() => log4netSettings.BaseDirectory, Is.NotNull);
            Guard.Argument(() => log4netSettings.Log4netConfigFileName, Is.NotNull);
            Guard.Argument(() => log4netSettings.Log4netFileName, Is.NotNull);

            var log4netConfigFilePath = Path.IsPathRooted(log4netSettings.Log4netConfigFileName)
                ? log4netSettings.Log4netConfigFileName
                : Path.Combine(log4netSettings.BaseDirectory, log4netSettings.Log4netConfigFileName);

            if (File.Exists(log4netConfigFilePath))
            {
                var fileAppender = GetFileAppender(Log4netAppenderName);

                string logFileSource = (fileAppender != null) ? fileAppender.File : null;

                XmlConfigurator.Configure(new FileInfo(log4netConfigFilePath));
                fileAppender = GetFileAppender(Log4netAppenderName);

                if (fileAppender != null)
                {
                    var logFilename = CalculationLogFilename(log4netSettings, log4netConfigFilePath);

                    if (string.IsNullOrEmpty(logFilename))
                    {
                        throw new Exception(ErrorCalculationLogFilename);
                    }

                    var originalAppenderFile = fileAppender.File;

                    fileAppender.File = !string.IsNullOrEmpty(logFileSource) ?
                                        logFilename.Replace(Path.GetFileName(logFilename), Path.GetFileName(log4netSettings.Log4netFileName)) : logFilename;

                    fileAppender.ActivateOptions();

                    if (!string.IsNullOrEmpty(originalAppenderFile) && (originalAppenderFile != fileAppender.File) &&
                        File.Exists(originalAppenderFile))
                    {
                        File.Delete(originalAppenderFile);
                    }

                    if (!string.IsNullOrEmpty(logFileSource) && (logFileSource != fileAppender.File) && File.Exists(logFileSource))
                    {
                        File.Delete(logFileSource);
                    }
                }
            }
        }

        private string Log4netAppenderName
        {
            get
            {
                if (log4netAppenderName == null)
                {
                    log4netAppenderName = ConfigurationManager.AppSettings[ConstLog4netAppenderName];

                    if (string.IsNullOrEmpty(log4netAppenderName))
                    {
                        throw new ArgumentException(Log4netAppenderNameError);
                    }
                }
                return log4netAppenderName;
            }
        }

        [ExcludeFromCodeCoverage]
        private string CalculationLogFilename(Log4netSettings log4netSettings, string log4netConfigFilePath)
        {
            using (XmlReader reader = XmlReader.Create(log4netConfigFilePath))
            {
                while (reader.Read())
                {
                    if (reader.Name == XmlElementFile && reader.NodeType == XmlNodeType.Element && reader.HasAttributes)
                    {
                        string fileSource = reader.GetAttribute(XmlAttributeValue);

                        if (string.IsNullOrEmpty(fileSource))
                        {
                            throw new Exception(ErrorCalculationLogFilename);
                        }

                        return Path.IsPathRooted(fileSource) ? fileSource : Path.Combine(log4netSettings.BaseDirectory, fileSource);
                    }
                }
            }

            return null;
        }

        [ExcludeFromCodeCoverage]
        private FileAppender GetFileAppender(string name)
        {
            return ((Hierarchy)LogManager.GetRepository()).Root.Appenders.OfType<FileAppender>().FirstOrDefault(x => x.Name == name);
        }
    }
}
