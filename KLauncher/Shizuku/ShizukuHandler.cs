using Android.OS;
using Android.Text;
using Java.Lang;

namespace KLauncher
{
    public sealed class ShizukuHandler : Handler
    {
        private ShizukuInstance Outer { get; }
        public ShizukuHandler(ShizukuInstance instance)
        {
            Outer = instance;
        }
        public override void HandleMessage(Message msg)
        {
#pragma warning disable CS0618 
            var result = msg.What == 1 ? Html.FromHtml(msg.Obj + "<br>").ToString() : String.ValueOf(msg.Obj);
            Outer.OutputMsg.Append(result);
#pragma warning restore CS0618 
        }
    }
}