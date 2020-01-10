using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ShortLink.Models;
using ShortLink.sakila;

namespace ShortLink.Controllers
{
    public class HomeController : Controller
    {
        private readonly shortlinkContext _context;
        public HomeController(shortlinkContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        private readonly ILogger<HomeController> _logger;

        public IActionResult Index()
        {
            ViewData["ShortLink"] = "";
            ViewData["SavingResult"]= "";
            return View();
        }
        //вывод списка ссылок
        public IActionResult Links()
        {
            List<Fulltable> fulltable = _context.Fulltable
                .Where(b => b.Deleted == 0)
                .OrderBy(b => b.Date)
                .ToList();
            return View(fulltable);
        }
        //форма редактирования ссылки
        public IActionResult Link(int id)
        {
            List<Fulltable> fulltable = _context.Fulltable
                .Where(b => b.Id == id)
                .ToList();
            ViewData["saved"] = "";
            return View(fulltable[0]);
        }
        //обработка и сохранение изменений
        public IActionResult LinkSaved(int id, string LongLinkText, string ShortLinkText)
        {
            var changedLine = _context.Linktable
                 .Where(b => b.Id == id)
                 .First();
            changedLine.Longlink = LongLinkText;
            changedLine.Shortlink = ShortLinkText;
            changedLine.Date = DateTime.Now;
            _context.SaveChanges();
            List < Fulltable > fulltable = _context.Fulltable
                .Where(b => b.Id == id)
                .ToList();
            ViewData["saved"] = "Изменения сохранены в " + DateTime.Now.TimeOfDay.ToString("hh\\:mm");
            return View("Link",fulltable[0]);
        } 
            //уведомление об удаленной ссылке
            public IActionResult Deleted (int id)
        {
            var deletedLine = _context.Linktable
                .Where(b => b.Id == id)
                .First();
            deletedLine.Deleted = 1;
            _context.SaveChanges();
            ViewData["id"] = id.ToString();
            return View();
        }
        //создание укороченной ссылки
        public IActionResult CutLink(string LongLink)
        {
            List<Fulltable> result = _context.Fulltable
                .Where(b => b.Longlink == LongLink)
                .ToList();
            if (result.Count() > 0) return View("Link", result[0]);
            else  ViewData["ShortLink"] = LinkCuting(LongLink);
            return View("Index");
        }
        //переадресация
        [Route("/{code}")]
        public IActionResult Redir(string code)
        {
            var result = _context.Linktable
                .Where(c => c.Shortlink.Contains(code)) 
                .First();
            int i = result.Id;
            var logItem = _context.Log
               .Where(b => b.IdLink == i)
                .First();
            logItem.Count++;
            _context.SaveChanges();
            return Redirect(result.Longlink);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        //обрезка
        public string LinkCuting (string longLink)
        {
            string htp = Request.IsHttps ? "https://" : "http://";
             htp   += Request.Host.Value + "/";
            int maxlength = htp.Length+5;
            if (longLink.Length <= maxlength) {
                @ViewData["SavingResult"] = "Строка и так короткая, преобразование не требуется";
                return longLink; 
            }
            int linkLength = longLink.Length > maxlength ? maxlength : longLink.Length;
            char[] chars = "!0123456789@ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".ToCharArray();
            StringBuilder shortLink = new StringBuilder(maxlength);
            Random random = new Random();
            bool haveSame;
                do
                {
                shortLink.Clear();
                shortLink.Append(htp);
                    do
                    {
                    shortLink.Append(chars[random.Next(0, chars.Length - 1)]);//never use bitwise opertions on server!
                    }
                    while (shortLink.Length <= linkLength);
                    haveSame = _context.Linktable
                        .Select(b => b.Shortlink)
                        .Contains(shortLink.ToString());
                } while (haveSame);
            AddItem(longLink, shortLink.ToString());
            @ViewData["SavingResult"] = "Строка укорочена и записана в базу";

            return shortLink.ToString();
        }
        //сохранение
        public void AddItem( string longLink, string shortLink)
        {
            Linktable newLine = new Linktable();
            newLine.Longlink = longLink;
            newLine.Shortlink = shortLink.ToString();
            newLine.Date = DateTime.Now;
            newLine.Deleted = 0;
            _context.Linktable.Add(newLine);
            _context.SaveChanges();
        }
    }
}
