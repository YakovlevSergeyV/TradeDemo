namespace GlobalSpace.Common.Dal
{
    using System;
    using System.Data;
    using GlobalSpace.Common.Dal.Abstract;

    /// <summary>
    /// Небольшой Helper-класс, позволяющий упростить семантику однократного обращения к БД с открытием транзакации и без открытия транзакции
    /// </summary>
    public static class ExecutionContextHelper
    {
        public static void Execute(IExecutionContextFactory factory, Action<IExecutionContext> action)
        {
            if (factory == null) throw new ArgumentNullException("factory");
            if (action == null) throw new ArgumentNullException("action");

            using (var executionContext = factory.Create())
            {
                executionContext.Open();

                action.Invoke(executionContext);

                executionContext.Close();
            }
        }

        public static T Execute<T>(IExecutionContextFactory factory, Func<IExecutionContext, T> action)
        {
            if (factory == null) throw new ArgumentNullException("factory");
            if (action == null) throw new ArgumentNullException("action");

            using (var executionContext = factory.Create())
            {
                executionContext.Open();

                return action.Invoke(executionContext);
            }
        }

        public static void ExecuteTran(IExecutionContextFactory factory, Action<IExecutionContext> action)
        {
            if (factory == null) throw new ArgumentNullException("factory");
            if (action == null) throw new ArgumentNullException("action");

 
            using (var executionContext = factory.Create())
            {
                executionContext.Open();
                executionContext.BeginTransaction();

                try
                {
                    action.Invoke(executionContext);
                }
                catch
                {
                    executionContext.Rollback();
                    executionContext.Close();
                    throw;
                }

                executionContext.Commit();
                executionContext.Close();
            }
        }

        public static void ExecuteTran(IExecutionContextFactory factory, IDbCommand command, Action<IExecutionContext> action)
        {
            if (factory == null) throw new ArgumentNullException("factory");
            if (action == null) throw new ArgumentNullException("action");

            using (var executionContext = factory.Create())
            {
                executionContext.Open();
                executionContext.BeginTransaction();
                executionContext.PrepareCommand(command);
                try
                {
                    action.Invoke(executionContext);
                }
                catch
                {
                    executionContext.Rollback();
                    executionContext.Close();
                    throw;
                }

                executionContext.Commit();
                executionContext.Close();
            }
        }
    }
}
