﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using domaine.entities;
using Newtonsoft.Json;
using web.App_Start;
using web.Util;

namespace web.Controllers
{
    public class productsController : Controller
    {
        private SWModel db = new SWModel();

        // GET: products
        public ActionResult Index()
        {
            user u = (user)Session["user"];
            if (u != null)
            {
                if (u.role == "ROLE_AGENT")
                {
                    var product = db.product.Include(p => p.sousCategorieProd);
                    return View(product.ToList().Where(pr=>pr.userId ==u.id));
                }

            }

            return new HttpStatusCodeResult(403);
        }

        // GET: products/Details/5
        public ActionResult Details(int? id)
        {
            user u = (user)Session["user"];
            if (u != null)
            {
                if (u.role == "ROLE_AGENT")
                {

                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    product product = db.product.Find(id);
                    if (product == null)
                    {
                        return HttpNotFound();
                    }
                    return View(product);
                }

            }

            return new HttpStatusCodeResult(403);
        }

        // GET: products/Create
        public ActionResult Create()
        {
            user u = (user) Session["user"];
            if (u != null)
            {
                if (u.role == "ROLE_AGENT")
                {
                    
                    ViewBag.sousCategorieProdId = new SelectList(db.souscategories, "Id", "Libelle");
                    return View();
                }
                
            }

            return new HttpStatusCodeResult(403);
        }

        // POST: products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Reference,Description,Marque,Modele,Name,Photo,Quantity,Color,Size,price,sousCategorieProdId,userId")] product product,HttpPostedFileBase File)
        {



            if (ModelState.IsValid)
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Config.URL + "badword");
                request.Method = "POST";
                request.Headers.Add("text", product.Description);
                using (var reader =new StreamReader(request.GetResponse().GetResponseStream(),Encoding.UTF8))
                {
                    string responsetext = reader.ReadToEnd();
                    MessageResponse msg = JsonConvert.DeserializeObject<MessageResponse>(responsetext);
                    if (msg.Code == 0)
                    {
                        if (File.ContentLength > 0)
                        {
                            string _FileName = Path.GetFileName(new Random().Next().ToString() + File.FileName);
                            var path = Path.Combine(Server.MapPath("~/Content/Upload"), _FileName);

                            File.SaveAs(path);
                            product.Photo= _FileName;
                        }
                        user u = (user)Session["user"];
                        product.userId = u.id;
                        db.product.Add(product);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        
                        List<string> badworsList =JsonConvert.DeserializeObject <List<string>>(msg.Message.ToString());
                     

                        return RedirectToAction("Create",new {msg=JsonConvert.SerializeObject(badworsList)});
                    }
                }

                
            }

            ViewBag.sousCategorieProdId = new SelectList(db.souscategories, "Id", "Libelle", product.sousCategorieProdId);
            return View(product);
        }

        // GET: products/Edit/5
        public ActionResult Edit(int? id)
        {
            user u = (user)Session["user"];
            if (u != null)
            {
                if (u.role == "ROLE_AGENT")
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    product product = db.product.Find(id);
                    if (product == null)
                    {
                        return HttpNotFound();
                    }
                    ViewBag.sousCategorieProdId = new SelectList(db.souscategories, "Id", "Libelle", product.sousCategorieProdId);
                    return View(product);
                }

            }

            return new HttpStatusCodeResult(403);
        }

        // POST: products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Reference,MarquecategorieProd,Description,Marque,Modele,Name,Photo,Quantity,Color,Size,price,sousCategorieProdId,userId")] product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.sousCategorieProdId = new SelectList(db.souscategories, "Id", "Libelle", product.sousCategorieProdId);
            return View(product);
        }

        // GET: products/Delete/5
        public ActionResult Delete(int? id)
        {
            user u = (user)Session["user"];
            if (u != null)
            {
                if (u.role == "ROLE_AGENT")
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    product product = db.product.Find(id);
                    if (product == null)
                    {
                        return HttpNotFound();
                    }
                    return View(product);

                }

            }

            return new HttpStatusCodeResult(403);
        }

        // POST: products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            product product = db.product.Find(id);
            db.product.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
