namespace GlobalSpace.Common.Dal.SQLite.Model
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using GlobalSpace.Common.Dal.Abstract;
    using GlobalSpace.Common.Dal.SQLite.Abstract;
    using GlobalSpace.Common.Guardly;

    public abstract class DbBase
    {
        private readonly ICommandsDb _commandsDb;

        protected DbBase(IExecutionContextFactory executionContextFactory, ICommandsDb commandsDb)
        {
            Guard.Argument(() => commandsDb, Is.NotNull);
            Guard.Argument(() => executionContextFactory, Is.NotNull);

            ExecutionContextFactory = executionContextFactory;
            _commandsDb = commandsDb;

            ExecutionContextHelper.Execute(
                ExecutionContextFactory,
                context => CreateTable(context, _commandsDb.CreateAdditionalInfoTable));

            // Записать версию схемы базы данных
            WriteAdditionalInfo(
                () => CreateAdditionalInfoReader(
                    new Dictionary<string, string>
                    {
                        { AdditionalInfoKeys.SchemaVersion, _commandsDb.Version.ToString(4) }
                    }));
        }

        public IExecutionContextFactory ExecutionContextFactory { get; private set; }

        public void WriteAdditionalInfo(Func<IDataReader> dataReaderFunction)
        {
            ExecutionContextHelper.ExecuteTran(
                ExecutionContextFactory,
                context =>
                {
                    using (var dataReader = dataReaderFunction.Invoke())
                    {
                        // Вставить дополнительную информацию
                        while (dataReader.Read())
                        {
                            using (var cmd = _commandsDb.InsertAdditionalParam(
                                dataReader.GetString(0),
                                dataReader.GetString(1)))
                            {
                                context.ExecuteNonQuery(cmd);
                            }
                        }
                    }
                });
        }

        protected void CreateTable(IExecutionContext executionContext, Func<IDbCommand> createCommandFunc)
        {
            using (var command = createCommandFunc.Invoke())
            {
                executionContext.ExecuteNonQuery(command);
            }
        }

        protected IDataReader CreateAdditionalInfoReader(IEnumerable<KeyValuePair<string, string>> dictionary)
        {
            var dataTable = new DataTable();

            dataTable.Columns.Add("Key", typeof(string));
            dataTable.Columns.Add("Value", typeof(string));

            foreach (var kv in dictionary)
            {
                dataTable.Rows.Add(kv.Key, kv.Value);
            }

            return dataTable.CreateDataReader();
        }
    }
}
