using Java.Lang;
using SQLite;

namespace KLauncher.Libs.Models
{
    [Table("AppItem")]
    public sealed class AppItem : Object
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Icon { get; set; }
        public string DisplayName { get; set; }
        public string PackageName { get; set; }
        public long VersionCode { get; set; }
        public bool IsVisable { get; set; }
        public bool IsSystem { get; set; }
    }
}