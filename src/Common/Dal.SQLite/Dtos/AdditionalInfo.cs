
namespace GlobalSpace.Common.Dal.SQLite.Dtos
{
    using GlobalSpace.Common.Dal.Abstract;

    public class AdditionalInfo
    {
        public string InfoKey { get; set; }
        public string InfoValue { get; set; }

        public static AdditionalInfo Load(IReaderWrapper wrapper)
        {
            var obj = new AdditionalInfo();
            obj.LoadInner(wrapper);
            return obj;
        }

        protected void LoadInner(IReaderWrapper wrapper)
        {
            InfoKey = wrapper.Read<string>("InfoKey");
            InfoValue = wrapper.Read<string>("InfoValue");
        }
    }
}
