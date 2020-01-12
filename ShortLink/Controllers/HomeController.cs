using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ShortLink.Models;

namespace ShortLink.Controllers
{
    public class HomeController : Controller
    {
        LinkMade linkscontext = new LinkMade();
        public IActionResult Index()
        {
            ViewData["ShortLink"] = "";
            ViewData["SavingResult"]= "";
            return View();
        }
        //вывод списка ссылок
        public IActionResult Links()
        {
            List<LinkTable> fulltable = linkscontext.GetAll();
            return View(fulltable);
        }
        ////форма редактирования ссылки
        public IActionResult Link(string id)
        {
            List<LinkTable> fulltable = linkscontext.GetById(id);
            return View(fulltable[0]);
        }
        //обработка и сохранение изменений
        public async Task<IActionResult> LinkSaved(string id, string LongLinkText, string ShortLinkText)
        {
            LinkTable newItem = new LinkTable();
            newItem.Id = id;
            newItem.Longlink = LongLinkText;
            newItem.Shortlink = ShortLinkText;
            newItem.Date = DateTime.Now;
            await linkscontext.Update(newItem);
            ViewData["saved"] = "Изменения сохранены в " + DateTime.Now.TimeOfDay.ToString("hh\\:mm");
            List<LinkTable> fulltable = linkscontext.GetById(id);
            return View("Link", fulltable[0]);
        }
        //уведомление об удаленной ссылке
        public async Task<IActionResult> Deleted(string id)
        {
            await linkscontext.Remove(id);
            ViewData["id"] = id.ToString();
            return View();
        }
        //создание укороченной ссылки
        public async Task<IActionResult> CutLink(string LongLink)
        {
            var result = await linkscontext.GetShortLink(LongLink);
            if (result.Count() > 0) return View("Link", result[0]);
            else ViewData["ShortLink"] = LinkCuting(LongLink);
            return View("Index");
        }

        //public string GetByLongLink(string LL)
        //{
        //    List<Lin> result = linkscontext.GetShortLink(LL);
        //    return result[0].Shortlink;
        //}

        ////переадресация
        [Route("/{code}")]
        public IActionResult Redir(string code)
        {
            string link = Request.Scheme + "://" + Request.Host.Value + "/"+ code;
            List<LinkTable> result = linkscontext.GetLongLink(link).Result;
            link = result[0].Longlink;
            return Redirect(link);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        ////обрезка
        public string LinkCuting(string longLink)
        {
            string htp = Request.Scheme +"://"+ Request.Host.Value + "/";
            int maxlength = htp.Length + 5;
            if (longLink.Length <= maxlength)
            {
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
                haveSame = linkscontext.GetByShortLink(shortLink.ToString());
            } while (haveSame);
            AddItem(longLink, shortLink.ToString());
            @ViewData["SavingResult"] = "Строка укорочена и записана в базу";

            return shortLink.ToString();
        }
        ////сохранение
        public async void AddItem(string longLink, string shortLink)
        {
            LinkTable newItem = new LinkTable();
            newItem.Longlink = longLink;
            newItem.Shortlink = shortLink.ToString();
            newItem.Date = DateTime.Now;
            await linkscontext.Create(newItem);
        }
    }
}
