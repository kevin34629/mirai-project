using print;
namespace login
{
    public struct Administrator
    {
        public string id;
        public string name;
    }
    public class Administrators
    {
        private static readonly List<Administrator> administrators = new();
        public Administrators() 
        {
            //root用户
            Administrator root = new();
            root.name = "kevin";
            root.id = "3462951477";
            administrators.Add(root);
            //初始化列表
            using StreamReader file = new("./login", true);
            string? line;
            while ((line = file.ReadLine()) != null)
            {
                string[] info = line.Split(' ');
                Administrator p = new();
                p.name = info[0];
                p.id = info[1];
                administrators.Add(p);
            }
        }
        //检验是否为root
        public static bool Isroot(string id)
        {
            bool result = false;
            foreach (Administrator administrator in administrators)
            {
                if (id == administrator.id)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }
        //添加管理员
        async public static void Add(string name,string id)
        {
            Administrator administrator = new();
            administrator.name = name;
            administrator.id = id;
            administrators.Add(administrator);
            await using (StreamWriter file = new("./login",true)) { file.WriteLine("{0} {1}", name, id); }
            Print.Printblueline("写入完毕");
        }
        //删除管理员
        async public static void Delete(string name)
        {
            Administrator delete=new();
            foreach (Administrator p in administrators) { if (p.name == name) { delete = p; } }
            administrators.Remove(delete);
            List<Administrator> list = new();
            using (StreamReader file = new("./login", true)) 
            {
                string? line;
                while ((line = file.ReadLine()) != null) 
                {
                    string[] info = line.Split(' ');
                    if (info[0] == name) { continue; }
                    Administrator p = new();
                    p.name = info[0];
                    p.id = info[1];
                    list.Add(p);
                } 
            }
            await using (StreamWriter file = new("./login", false)) { foreach (Administrator p in list) { file.WriteLine("{0} {1}", p.name, p.id); } }
            Print.Printblueline("删除完毕");
        }
        //查看管理员列表
        public static List<Administrator> List() { return administrators; }
    }
}