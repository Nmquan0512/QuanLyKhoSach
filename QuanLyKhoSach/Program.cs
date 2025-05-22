using System;
using System.Collections.Generic;
using System.Linq;

class Book
{
    public int ID { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public string Genre { get; set; }
    public int Quantity { get; set; }
}

class Program
{
    static List<Book> books = new List<Book>();
    static Stack<string> history = new Stack<string>();
    static Dictionary<string, List<int>> userBorrows = new Dictionary<string, List<int>>();
    static Dictionary<int, Queue<string>> waitingList = new Dictionary<int, Queue<string>>();
    static HashSet<string> genres = new HashSet<string>();

    static void Main()
    {
        bool running = true;
        while (running)
        {
            Console.WriteLine("\n--- MENU ---");
            Console.WriteLine("1. Them sach moi");
            Console.WriteLine("2. Tim sach theo the loai/ten tac gia");
            Console.WriteLine("3. Phan loai sach theo the loai");
            Console.WriteLine("4. Muon sach");
            Console.WriteLine("5. Tra sach");
            Console.WriteLine("6. Hien thi nguoi dang muon sach");
            Console.WriteLine("7. Thong ke sach");
            Console.WriteLine("8. Thoat");
            Console.Write("Chon chuc nang: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1": ThemSach(); break;
                case "2": TimKiemSach(); break;
                case "3": PhanLoaiSach(); break;
                case "4": XuLyMuonSach(); break;
                case "5": XuLyTraSach(); break;
                case "6": HienThiNguoiMuon(); break;
                case "7": ThongKe(); break;
                case "8": running = false; break;
                default: Console.WriteLine("Lua chon khong hop le."); break;
            }
        }
    }

    static void ThemSach()
    {
        Console.Write("Nhap ID sach: ");
        if (!int.TryParse(Console.ReadLine(), out int id)) return;
        Console.Write("Nhap ten sach: ");
        string title = Console.ReadLine();
        Console.Write("Nhap ten tac gia: ");
        string author = Console.ReadLine();
        Console.Write("Nhap the loai: ");
        string genre = Console.ReadLine();
        Console.Write("Nhap so luong: ");
        if (!int.TryParse(Console.ReadLine(), out int quantity)) return;

        var book = books.FirstOrDefault(b => b.ID == id);
        if (book != null)
        {
            book.Quantity += quantity;
            Console.WriteLine("Sach da ton tai, tang so luong.");
        }
        else
        {
            books.Add(new Book { ID = id, Title = title, Author = author, Genre = genre, Quantity = quantity });
            genres.Add(genre);
            Console.WriteLine("Them sach thanh cong.");
        }
    }

    static void TimKiemSach()
    {
        Console.Write("Nhap the loai hoac ten tac gia: ");
        string keyword = Console.ReadLine().ToLower();
        var result = books
            .Where(b => b.Genre.ToLower().Contains(keyword) || b.Author.ToLower().Contains(keyword))
            .Select(b => $"{b.ID} - {b.Title} - {b.Author} - {b.Genre} - SL: {b.Quantity}");
        foreach (var b in result) Console.WriteLine(b);
    }

    static void PhanLoaiSach()
    {
        var grouped = books.GroupBy(b => b.Genre);
        foreach (var group in grouped)
        {
            int total = group.Sum(b => b.Quantity);
            Console.WriteLine($"{group.Key}: {total} cuon");
        }
    }

    static void XuLyMuonSach()
    {
        Console.Write("Nhap ten nguoi muon: ");
        string user = Console.ReadLine();
        Console.Write("Nhap ID sach muon: ");
        if (!int.TryParse(Console.ReadLine(), out int id)) return;

        var book = books.FirstOrDefault(b => b.ID == id);
        if (book == null) { Console.WriteLine("Khong tim thay sach."); return; }

        if (book.Quantity > 0)
        {
            book.Quantity--;
            if (!userBorrows.ContainsKey(user)) userBorrows[user] = new List<int>();
            userBorrows[user].Add(id);
            history.Push($"Muon: {id} - {user}");
            Console.WriteLine("Muon sach thanh cong.");
        }
        else
        {
            if (!waitingList.ContainsKey(id)) waitingList[id] = new Queue<string>();
            waitingList[id].Enqueue(user);
            Console.WriteLine("Sach het. Da them vao hang cho.");
        }
    }

    static void XuLyTraSach()
    {
        Console.Write("Nhap ten nguoi tra: ");
        string user = Console.ReadLine();
        Console.Write("Nhap ID sach tra: ");
        if (!int.TryParse(Console.ReadLine(), out int id)) return;

        var book = books.FirstOrDefault(b => b.ID == id);
        if (book == null || !userBorrows.ContainsKey(user) || !userBorrows[user].Contains(id))
        {
            Console.WriteLine("Thong tin khong chinh xac.");
            return;
        }

        userBorrows[user].Remove(id);
        if (userBorrows[user].Count == 0) userBorrows.Remove(user);

        if (waitingList.ContainsKey(id) && waitingList[id].Count > 0)
        {
            string nextUser = waitingList[id].Dequeue();
            if (!userBorrows.ContainsKey(nextUser)) userBorrows[nextUser] = new List<int>();
            userBorrows[nextUser].Add(id);
            history.Push($"Muon (tu cho): {id} - {nextUser}");
            Console.WriteLine($"Nguoi cho ({nextUser}) da duoc muon sach.");
        }
        else
        {
            book.Quantity++;
        }

        history.Push($"Tra: {id} - {user}");
        Console.WriteLine("Tra sach thanh cong.");
    }

    static void HienThiNguoiMuon()
    {
        var join = from kv in userBorrows
                   from id in kv.Value
                   join b in books on id equals b.ID
                   select new { User = kv.Key, Book = b.Title };

        foreach (var item in join)
        {
            Console.WriteLine($"{item.User} dang muon: {item.Book}");
        }
    }

    static void ThongKe()
    {
        Console.WriteLine("Top 3 sach co so luong nhieu nhat:");
        var top = books.OrderByDescending(b => b.Quantity).Take(3);
        foreach (var b in top)
        {
            Console.WriteLine($"{b.Title} - SL: {b.Quantity}");
        }

        Console.WriteLine("\nCac the loai da co:");
        foreach (var g in genres)
        {
            Console.WriteLine("- " + g);
        }

        Console.WriteLine("\nLich su muon/tra:");
        foreach (var entry in history)
        {
            Console.WriteLine(entry);
        }
    }
}
