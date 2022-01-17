using Mirai.Net.Data.Messages.Concretes;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Sessions;
using Mirai.Net.Sessions.Http.Managers;
using System.Reactive.Linq;
using login;
using print;
using System.Text.RegularExpressions;
//加载bot
using var bot = new MiraiBot
{
    Address = "localhost:8080",
    QQ = "3577873667",
    VerifyKey = "164352pkw"
};
await bot.LaunchAsync();
//设置信号
Print.Printredline("程序正在运行......");
var signal = new ManualResetEvent(false);
//加载管理员列表
Administrators administrators = new();
//获取操作指令
async void operation(string str, string id ,string message) 
{
    //停止程序
    if (str == "/exit" && Administrators.Isroot(id)) { signal.Set(); }
    //回复success
    if (str == "/test") 
    { 
        await MessageManager.QuoteFriendMessageAsync(id, message, "success");
        Print.Printblueline("已回复success");
    }
    //查看管理员列表
    if (str == "/administrators" && Administrators.Isroot(id))
    {
        string list = "";
        foreach (Administrator p in Administrators.List()) { list = list + p.name + " " + p.id + "\n"; }
        await MessageManager.SendFriendMessageAsync(id, list);
        Print.Printblueline("已发送列表");
    }
    //add
    if (Regex.IsMatch(str, @"/add\s\S+\s\d+") && Administrators.Isroot(id)) 
    {
        string[] info=str.Split(' ');
        Administrators.Add(info[1], info[2]);
    }
    //delete
    if (Regex.IsMatch(str, @"/delete\s\S+") && Administrators.Isroot(id))
    {
        string[] info = str.Split(' ');
        Administrators.Delete(info[1]);
    }
}
//解析消息链
bot.MessageReceived
.OfType<FriendMessageReceiver>()
.Subscribe(receiver =>
{
    string senderName = receiver.Sender.NickName;   //获取昵称
    string senderid = receiver.Sender.Id;   //获取账号
    string messageid = receiver.MessageChain.OfType<SourceMessage>().First().MessageId;
    //解析文本
    Print.Printyellowline($"{senderName}({senderid})");
    try
    {
        string message = receiver.MessageChain.OfType<PlainMessage>().First().Text;
        Console.Write($"{message} ");
        Print.Printgreenline("(Text)");
        operation(message, senderid, messageid);
    }
    catch { }
    //解析表情
    try
    {
        var emotions = receiver.MessageChain.OfType<FaceMessage>().ToList();
        if(emotions.Count != 0) 
        {
            foreach (var i in emotions) { Console.Write($"[{i.Name}] "); }
            Print.Printgreenline("(emotions)");
        }
    }
    catch { }
    //解析图片
    try
    {
        var images = receiver.MessageChain.OfType<ImageMessage>().ToList();
        if(images.Count != 0) 
        {
            foreach (var i in images) {  Console.WriteLine($"{ i.Url}"); }
            //Console.WriteLine("(images)");
            Print.Printgreenline("(images)");
        }
    }
    catch { }
    //解析戳一戳事件
    try 
    {
        var poke = receiver.MessageChain.OfType<PokeMessage>().ToList();
        if(poke.Count != 0)
        { 
            MessageManager.SendFriendMessageAsync(senderid, "？");
            Print.Printgreenline("(poke)");;
            Print.Printblueline("已回复:？");
        }
    }
    catch { }
});
//线程阻塞
signal.WaitOne();