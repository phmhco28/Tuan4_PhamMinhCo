﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuan4_PhamMinhCo.Models;

namespace Tuan4_PhamMinhCo.Controllers
{
    public class SachController : Controller
    {
        // GET: Sach

        MyDataDataContext data = new MyDataDataContext();

        public ActionResult ListSach()
        {
            var all_sach = from tt in data.Saches select tt;
            return View(all_sach);
        }

        public ActionResult Detail(int id)
        {
            var D_sach = data.Saches.Where(p => p.masach == id).FirstOrDefault();
            TempData["value"] = "test";
            return View(D_sach);
        }

        public ActionResult Create()
        {
            var list = data.TheLoais.ToList();
            ViewBag.listCategory = new SelectList(list, "maloai", "tenloai");
            return View();
        }
        [HttpPost]
        public ActionResult Create(FormCollection collection, Sach s)
        {
            var E_tensach = collection["tensach"];
            var E_hinh = collection["Hình"];
            var E_giaban = Convert.ToDecimal(collection["giaban"]);
            var E_ngaycapnhat = Convert.ToDateTime(collection["ngaycapnhat"]);
            var E_soluongton = Convert.ToInt32(collection["soluongton"]);
            if (string.IsNullOrEmpty(E_tensach))
            {
                ViewData["Error"] = "Don't Empty";
            }
            else
            {
                s.tensach = E_tensach.ToString();
                s.hinh = E_hinh.ToString();
                s.giaban = E_giaban;
                s.ngaycapnhat = E_ngaycapnhat;
                s.soluongton = E_soluongton;
                data.Saches.InsertOnSubmit(s);
                data.SubmitChanges();
                return RedirectToAction("ListSach");
            }
            return this.Create();

        }

        public ActionResult Edit(int id)
        {
            var E_sach = data.Saches.First(m => m.masach == id);
            var list = data.TheLoais.ToList();
            ViewBag.listCategory = new SelectList(list, "maloai", "tenloai");
            return View(E_sach);
        }

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            var E_sach = data.Saches.First(m => m.masach == id);
            var E_tensach = collection["tensach"];
            var E_hinh = collection["hinh"];
            var E_giaban = Convert.ToDecimal(collection["giaban"]);
            var E_ngaycapnhat = Convert.ToDateTime(collection["ngaycatnhat"]);
            var E_soluongton = Convert.ToInt32(collection["soluongton"]);
            E_sach.masach = id;
            if (string.IsNullOrEmpty(E_tensach))
            {
                ViewData["Error"] = "Don't empty!";
            }
            else
            {
                E_sach.tensach = E_tensach;
                E_sach.hinh = E_hinh;
                E_sach.giaban = E_giaban;
                E_sach.ngaycapnhat = E_ngaycapnhat;
                E_sach.soluongton = E_soluongton;
                UpdateModel(E_sach);
                data.SubmitChanges();
                return RedirectToAction("ListSach");
            }
            return this.Edit(id);
        }

        public ActionResult Delete(int id)
        {
            var D_sach = data.Saches.First(m => m.masach == id);
            return View(D_sach);
        }
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            var D_sach = data.Saches.Where(m => m.masach == id).First();
            data.Saches.DeleteOnSubmit(D_sach);
            data.SubmitChanges();
            return RedirectToAction("ListSach");
        }

        public string ProcessUpload(HttpPostedFileBase file)
        {
            if (file == null)
            {
                return "";
            }
            file.SaveAs(Server.MapPath("~/Content/images/" + file.FileName));
            return "/Content/images/" + file.FileName;
        }

    }
}