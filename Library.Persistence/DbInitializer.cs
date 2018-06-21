using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Library.Persistence
{
    public static class DbInitializer
    {
        private static LibraryContext _context;

        public static void Initialize(LibraryContext context, string imageDirectory)
        {
            _context = context;
            _context.Database.EnsureCreated();

            if (_context.Books.Any())
            {
                return;
            }

            SeedBooks(imageDirectory);
            SeedLibrarians();
            SeedVolumes();
            SeedVisitors();
            SeedRents();
        }


        private static void SeedBooks(string imageDirectory)
        {
            // Ellenőrizzük, hogy képek könyvtára létezik-e.
            if (Directory.Exists(imageDirectory))
            {
                var books = new Book[]
                {
                    new Book {Title="Trónok harca",Author="George Martin",ReleaseYear=1996,ISBN="9789634470878",Picture=File.ReadAllBytes(imageDirectory+"\\game_of_thrones.png")},
                    new Book {Title="Muszasi I.",Author="Josikava Eidzsi",ReleaseYear=1935,ISBN="9789632278834",Picture=File.ReadAllBytes(imageDirectory+"\\muszasi.png")},
                    new Book {Title="Taikó I.",Author="Josikava Eidzsi",ReleaseYear=1937,ISBN="9789632278131",Picture=File.ReadAllBytes(imageDirectory+"\\taiko.png")},
                    new Book {Title="Vaják I.",Author="Andrzej Sapkowski",ReleaseYear=1993,ISBN="9789630810807",Picture=File.ReadAllBytes(imageDirectory+"\\vajak.png")},
                    new Book {Title="Harry Potter I.",Author="J.K.Rowling",ReleaseYear=1997,ISBN="9789633244548",Picture=File.ReadAllBytes(imageDirectory+"\\harry_potter.png")},
                    new Book {Title="A holtak vonulása",Author="Darren Shan",ReleaseYear=2010,ISBN="9789633100219",Picture=File.ReadAllBytes(imageDirectory+"\\a_holtak_vonulasa.png")},
                    new Book {Title="Vadölő",Author="J.F.Cooper",ReleaseYear=1823,ISBN="9789633490501",Picture=File.ReadAllBytes(imageDirectory+"\\vadolo.png")},
                    new Book {Title="A szél neve",Author="Patrick Rothfuss",ReleaseYear=2007,ISBN="9789636899332",Picture=File.ReadAllBytes(imageDirectory+"\\a_szel_neve.png")},
                    new Book {Title="Anna Karenina",Author="Lev Tolsztoj",ReleaseYear=1878,ISBN="9780143035008",Picture=File.ReadAllBytes(imageDirectory+"\\anna_karenina.png")},
                    new Book {Title="Az idő rövid története",Author="Stephen Hawking",ReleaseYear=1988,ISBN="9789632520384",Picture=File.ReadAllBytes(imageDirectory+"\\az_ido_rovid_tortenete.png")},
                    new Book {Title="Farkassziget",Author="Darren Shan",ReleaseYear=2005,ISBN="9789631197419",Picture=File.ReadAllBytes(imageDirectory+"\\farkassziget.png")},
                    new Book {Title="A kőszívű ember fiai",Author="Jókai Mór",ReleaseYear=1869,ISBN="9789630980647",Picture=File.ReadAllBytes(imageDirectory+"\\a_koszivu_ember_fiai.png")},
                    new Book {Title="Egri csillagok",Author="Gárdonyi Géza",ReleaseYear=1901,ISBN="9789633490983",Picture=File.ReadAllBytes(imageDirectory+"\\egri_csillagok.png")},
                    new Book {Title="A C++ programozási nyelv",Author="Bjarne Stroustrup",ReleaseYear=1985,ISBN="9789639301184",Picture=File.ReadAllBytes(imageDirectory+"\\a_cpp_programozasi_nyelv.png")},
                    new Book {Title="Game Programming using Qt",Author="Lorenz Haas",ReleaseYear=2016,ISBN="9781782168874",Picture=File.ReadAllBytes(imageDirectory+"\\game_programming_using_qt.png")},
                    new Book {Title="Büszkeség és balítélet",Author="Jane Austen",ReleaseYear=1813,ISBN="9789633412022",Picture=File.ReadAllBytes(imageDirectory+"\\buszkeseg_es_balitelet.png")},
                    new Book {Title="Elfújta a szél",Author="Margaret Mitchell",ReleaseYear=1936,ISBN="9789630796651",Picture=File.ReadAllBytes(imageDirectory+"\\elfujta_a_szel.png")},
                    new Book {Title="Bűn és bűnhődés",Author="Dosztojevszkij",ReleaseYear=1866,ISBN="9786155296918",Picture=File.ReadAllBytes(imageDirectory+"\\bun_es_bunhodes.png")},
                    new Book {Title="Száz év magány",Author="Gabriel García Márquez",ReleaseYear=1967,ISBN="9789631436167",Picture=File.ReadAllBytes(imageDirectory+"\\szaz_ev_magany.png")},
                    new Book {Title="Malevil",Author="Robert Merle",ReleaseYear=1972,ISBN="9789630796408",Picture=File.ReadAllBytes(imageDirectory+"\\malevil.png")},
                    new Book {Title="Pillangó",Author="Henri Charrière",ReleaseYear=1970,ISBN="9789639667938 ",Picture=File.ReadAllBytes(imageDirectory+"\\pillango.png")},
                    new Book {Title="Interjú a vámpírral",Author="Anne Rice",ReleaseYear=1976,ISBN="9639441821",Picture=File.ReadAllBytes(imageDirectory+"\\interju_a_vampirral.png")}
                };

                foreach (Book s in books)
                {
                    _context.Books.Add(s);
                }
                _context.SaveChanges();
            }
        }

        private static void SeedVolumes()
        {
            var volumes = new Volume[]
            {
                new Volume {BookId=1}, new Volume {BookId=1}, new Volume {BookId=1}, new Volume {BookId=1}, new Volume {BookId=1}, new Volume {BookId=2},
                new Volume {BookId=2}, new Volume {BookId=2}, new Volume {BookId=2}, new Volume {BookId=2}, new Volume {BookId=3}, new Volume {BookId=3},
                new Volume {BookId=3}, new Volume {BookId=4}, new Volume {BookId=4}, new Volume {BookId=4}, new Volume {BookId=4}, new Volume {BookId=4},
                new Volume {BookId=4}, new Volume {BookId=4}, new Volume {BookId=5}, new Volume {BookId=5}, new Volume {BookId=5}, new Volume {BookId=5},
                new Volume {BookId=5}, new Volume {BookId=5}, new Volume {BookId=5}, new Volume {BookId=6}, new Volume {BookId=6}, new Volume {BookId=6},
                new Volume {BookId=7}, new Volume {BookId=7}, new Volume {BookId=7}, new Volume {BookId=7}, new Volume {BookId=7}, new Volume {BookId=8},
                new Volume {BookId=8}, new Volume {BookId=8}, new Volume {BookId=9}, new Volume {BookId=10},new Volume {BookId=11},new Volume {BookId=11},
                new Volume {BookId=11}, new Volume {BookId=11}, new Volume {BookId=11}, new Volume {BookId=12}, new Volume {BookId=13}, new Volume {BookId=14},
                new Volume {BookId=15}, new Volume {BookId=16}, new Volume {BookId=17}, new Volume {BookId=18}, new Volume {BookId=19}, new Volume {BookId=20},
                new Volume {BookId=21}, new Volume {BookId=22}
            };

            foreach (Volume s in volumes)
            {
                _context.Volumes.Add(s);
            }
            _context.SaveChanges();
        }

        private static void SeedVisitors()
        {
            SHA512CryptoServiceProvider provider = new SHA512CryptoServiceProvider();

            var visitors = new Visitor[]
            {
                new Visitor {Name="zsiros b. ödön",Address="cink utca 20.",PhoneNumber="0600000",Password=provider.ComputeHash(Encoding.UTF8.GetBytes("asd"))},
                new Visitor {Name="kandisz nora",Address="kossuth utca 21.",PhoneNumber="062222",Password=provider.ComputeHash(Encoding.UTF8.GetBytes("aaa"))},
                new Visitor {Name="teszt elek",Address="rezes utca 3.",PhoneNumber="06111111",Password=provider.ComputeHash(Encoding.UTF8.GetBytes("sss"))},
                new Visitor {Name="egyip tomi",Address="pesti utca 11.",PhoneNumber="0613333",Password=provider.ComputeHash(Encoding.UTF8.GetBytes("ddd"))},
                new Visitor {Name="aloe vera",Address="proli utca 10.",PhoneNumber="0614444",Password=provider.ComputeHash(Encoding.UTF8.GetBytes("qqq"))}
            };

            foreach (Visitor s in visitors)
            {
                _context.Visitors.Add(s);
            }
            _context.SaveChanges();
        }

        private static void SeedRents()
        {
            var rents = new Rent[]
            {
                new Rent {StartDate=DateTime.Parse("2018-04-07"),EndDate=DateTime.Parse("2018-04-10"),IsActive=true,VolumeId=1,VisitorId=1},
                new Rent {StartDate=DateTime.Parse("2018-04-05"),EndDate=DateTime.Parse("2018-04-11"),IsActive=true,VolumeId=2,VisitorId=2},
                new Rent {StartDate=DateTime.Parse("2018-04-06"),EndDate=DateTime.Parse("2018-04-09"),IsActive=true,VolumeId=3,VisitorId=3},
                new Rent {StartDate=DateTime.Parse("2018-04-18"),EndDate=DateTime.Parse("2018-04-20"),IsActive=false,VolumeId=30,VisitorId=4},
                new Rent {StartDate=DateTime.Parse("2018-04-03"),EndDate=DateTime.Parse("2018-04-10"),IsActive=true,VolumeId=10,VisitorId=5},
                new Rent {StartDate=DateTime.Parse("2018-04-03"),EndDate=DateTime.Parse("2018-04-10"),IsActive=true,VolumeId=4,VisitorId=5},
                new Rent {StartDate=DateTime.Parse("2018-04-03"),EndDate=DateTime.Parse("2018-04-10"),IsActive=true,VolumeId=5,VisitorId=5},
                new Rent {StartDate=DateTime.Parse("2018-04-03"),EndDate=DateTime.Parse("2018-04-10"),IsActive=true,VolumeId=6,VisitorId=5},
                new Rent {StartDate=DateTime.Parse("2018-04-03"),EndDate=DateTime.Parse("2018-04-10"),IsActive=true,VolumeId=7,VisitorId=5},
                new Rent {StartDate=DateTime.Parse("2018-04-03"),EndDate=DateTime.Parse("2018-04-10"),IsActive=true,VolumeId=8,VisitorId=5},
                new Rent {StartDate=DateTime.Parse("2018-04-03"),EndDate=DateTime.Parse("2018-04-10"),IsActive=true,VolumeId=9,VisitorId=5},
                new Rent {StartDate=DateTime.Parse("2018-04-03"),EndDate=DateTime.Parse("2018-04-10"),IsActive=true,VolumeId=11,VisitorId=5},
                new Rent {StartDate=DateTime.Parse("2018-04-03"),EndDate=DateTime.Parse("2018-04-10"),IsActive=true,VolumeId=12,VisitorId=5},
                new Rent {StartDate=DateTime.Parse("2018-04-03"),EndDate=DateTime.Parse("2018-04-10"),IsActive=true,VolumeId=13,VisitorId=5}
                //new Rent {StartDate=DateTime.Parse("2018-04-03"),EndDate=DateTime.Parse("2018-04-10"),IsActive=true,VolumeId=14,VisitorId=5},
                //new Rent {StartDate=DateTime.Parse("2018-04-03"),EndDate=DateTime.Parse("2018-04-10"),IsActive=true,VolumeId=15,VisitorId=5},
                //new Rent {StartDate=DateTime.Parse("2018-04-03"),EndDate=DateTime.Parse("2018-04-10"),IsActive=true,VolumeId=16,VisitorId=5},
                //new Rent {StartDate=DateTime.Parse("2018-04-03"),EndDate=DateTime.Parse("2018-04-10"),IsActive=true,VolumeId=17,VisitorId=5},
                //new Rent {StartDate=DateTime.Parse("2018-04-03"),EndDate=DateTime.Parse("2018-04-10"),IsActive=true,VolumeId=18,VisitorId=5},
                //new Rent {StartDate=DateTime.Parse("2018-04-03"),EndDate=DateTime.Parse("2018-04-10"),IsActive=true,VolumeId=19,VisitorId=5},
                //new Rent {StartDate=DateTime.Parse("2018-04-03"),EndDate=DateTime.Parse("2018-04-10"),IsActive=true,VolumeId=20,VisitorId=5},
                //new Rent {StartDate=DateTime.Parse("2018-04-03"),EndDate=DateTime.Parse("2018-04-10"),IsActive=true,VolumeId=21,VisitorId=5},
                //new Rent {StartDate=DateTime.Parse("2018-04-03"),EndDate=DateTime.Parse("2018-04-10"),IsActive=true,VolumeId=22,VisitorId=5},
                //new Rent {StartDate=DateTime.Parse("2018-04-03"),EndDate=DateTime.Parse("2018-04-10"),IsActive=true,VolumeId=23,VisitorId=5},
                //new Rent {StartDate=DateTime.Parse("2018-04-03"),EndDate=DateTime.Parse("2018-04-10"),IsActive=true,VolumeId=24,VisitorId=5},
                //new Rent {StartDate=DateTime.Parse("2018-04-03"),EndDate=DateTime.Parse("2018-04-10"),IsActive=true,VolumeId=25,VisitorId=5},
                //new Rent {StartDate=DateTime.Parse("2018-04-03"),EndDate=DateTime.Parse("2018-04-10"),IsActive=true,VolumeId=26,VisitorId=5},
                //new Rent {StartDate=DateTime.Parse("2018-04-03"),EndDate=DateTime.Parse("2018-04-10"),IsActive=true,VolumeId=27,VisitorId=5},
                //new Rent {StartDate=DateTime.Parse("2018-04-03"),EndDate=DateTime.Parse("2018-04-10"),IsActive=true,VolumeId=28,VisitorId=5},
                //new Rent {StartDate=DateTime.Parse("2018-04-03"),EndDate=DateTime.Parse("2018-04-10"),IsActive=true,VolumeId=29,VisitorId=5},
                //new Rent {StartDate=DateTime.Parse("2018-04-03"),EndDate=DateTime.Parse("2018-04-10"),IsActive=true,VolumeId=30,VisitorId=5},
                //new Rent {StartDate=DateTime.Parse("2018-04-03"),EndDate=DateTime.Parse("2018-04-10"),IsActive=true,VolumeId=31,VisitorId=5},
                //new Rent {StartDate=DateTime.Parse("2018-04-03"),EndDate=DateTime.Parse("2018-04-10"),IsActive=true,VolumeId=32,VisitorId=5},
                //new Rent {StartDate=DateTime.Parse("2018-04-03"),EndDate=DateTime.Parse("2018-04-10"),IsActive=true,VolumeId=33,VisitorId=5},
                //new Rent {StartDate=DateTime.Parse("2018-04-03"),EndDate=DateTime.Parse("2018-04-10"),IsActive=true,VolumeId=34,VisitorId=5},
                //new Rent {StartDate=DateTime.Parse("2018-04-03"),EndDate=DateTime.Parse("2018-04-10"),IsActive=true,VolumeId=35,VisitorId=5},
                //new Rent {StartDate=DateTime.Parse("2018-04-03"),EndDate=DateTime.Parse("2018-04-10"),IsActive=true,VolumeId=36,VisitorId=5},
                //new Rent {StartDate=DateTime.Parse("2018-04-03"),EndDate=DateTime.Parse("2018-04-10"),IsActive=true,VolumeId=37,VisitorId=5},
                //new Rent {StartDate=DateTime.Parse("2018-04-03"),EndDate=DateTime.Parse("2018-04-10"),IsActive=true,VolumeId=38,VisitorId=5},
                //new Rent {StartDate=DateTime.Parse("2018-04-03"),EndDate=DateTime.Parse("2018-04-10"),IsActive=true,VolumeId=39,VisitorId=5},
                //new Rent {StartDate=DateTime.Parse("2018-04-03"),EndDate=DateTime.Parse("2018-04-10"),IsActive=true,VolumeId=40,VisitorId=5},
                //new Rent {StartDate=DateTime.Parse("2018-04-03"),EndDate=DateTime.Parse("2018-04-10"),IsActive=true,VolumeId=41,VisitorId=5},
                //new Rent {StartDate=DateTime.Parse("2018-04-03"),EndDate=DateTime.Parse("2018-04-10"),IsActive=true,VolumeId=42,VisitorId=5},
                //new Rent {StartDate=DateTime.Parse("2018-04-03"),EndDate=DateTime.Parse("2018-04-10"),IsActive=true,VolumeId=43,VisitorId=5},
                //new Rent {StartDate=DateTime.Parse("2018-04-03"),EndDate=DateTime.Parse("2018-04-10"),IsActive=true,VolumeId=44,VisitorId=5},
                //new Rent {StartDate=DateTime.Parse("2018-04-03"),EndDate=DateTime.Parse("2018-04-10"),IsActive=true,VolumeId=45,VisitorId=5},
                //new Rent {StartDate=DateTime.Parse("2018-04-03"),EndDate=DateTime.Parse("2018-04-10"),IsActive=true,VolumeId=46,VisitorId=5},
                //new Rent {StartDate=DateTime.Parse("2018-04-03"),EndDate=DateTime.Parse("2018-04-10"),IsActive=true,VolumeId=47,VisitorId=5},
                //new Rent {StartDate=DateTime.Parse("2018-04-03"),EndDate=DateTime.Parse("2018-04-10"),IsActive=true,VolumeId=48,VisitorId=5},
                //new Rent {StartDate=DateTime.Parse("2018-04-03"),EndDate=DateTime.Parse("2018-04-10"),IsActive=true,VolumeId=49,VisitorId=5},
                //new Rent {StartDate=DateTime.Parse("2018-04-03"),EndDate=DateTime.Parse("2018-04-10"),IsActive=true,VolumeId=50,VisitorId=5},
                //new Rent {StartDate=DateTime.Parse("2018-04-03"),EndDate=DateTime.Parse("2018-04-10"),IsActive=true,VolumeId=51,VisitorId=5},
                //new Rent {StartDate=DateTime.Parse("2018-04-03"),EndDate=DateTime.Parse("2018-04-10"),IsActive=true,VolumeId=52,VisitorId=5},
                //new Rent {StartDate=DateTime.Parse("2018-04-03"),EndDate=DateTime.Parse("2018-04-10"),IsActive=true,VolumeId=53,VisitorId=5},
                //new Rent {StartDate=DateTime.Parse("2018-04-03"),EndDate=DateTime.Parse("2018-04-10"),IsActive=true,VolumeId=54,VisitorId=5},
                //new Rent {StartDate=DateTime.Parse("2018-04-03"),EndDate=DateTime.Parse("2018-04-10"),IsActive=true,VolumeId=55,VisitorId=5},
                //new Rent {StartDate=DateTime.Parse("2018-04-03"),EndDate=DateTime.Parse("2018-04-10"),IsActive=true,VolumeId=56,VisitorId=5}
            };

            foreach (Rent s in rents)
            {
                _context.Rents.Add(s);
            }
            _context.SaveChanges();
        }

        private static void SeedLibrarians()
        {
            SHA512CryptoServiceProvider provider = new SHA512CryptoServiceProvider();

            var admins = new Librarian[]
            {
                new Librarian {Name="admin", Password=provider.ComputeHash(Encoding.UTF8.GetBytes("asd"))}
            };

            foreach (Librarian l in admins)
            {
                _context.Librarians.Add(l);
            }
            _context.SaveChanges();
        }
    }
}
