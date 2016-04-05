using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestTask_28._03._16_.Models;

namespace TestTask_28._03._16_.Controllers
{
    public class HomeController : Controller
    {
        DatabaseContext db = new DatabaseContext();

        [Authorize]
        public ActionResult Index()
        {
            //если пользователь выступает в роли администратора
            if(User.IsInRole("Admin"))                                  
            {
                //формируется список всех записей имеющихся в БД
                return View(db.Records.Include("Body").ToList());       
            }
            else
            {
                //иначе формируется список содержащий записи только текущего пользователся не имеющие флага гласящем об удалении записи пользователем
                List<Record> currentRecords = db.Records.Include("Body").Where(m => m.Author == User.Identity.Name && m.RemovedByUser == false).Select(m => m).ToList();
                return View(currentRecords);
            }
        }


        public ActionResult Add()
        {
            return View(new Record());
        }

        [HttpPost]
        public ActionResult Add(Record newRec)
        {
            //если обязательные поля формы заполненны корректно
            if (ModelState.IsValid)             
            {
                //если запись с таким ID уже существует в БД
                if (db.Records.Where(m => m.ID == newRec.ID).FirstOrDefault() != null)              
                {
                    Record temp = db.Records.Include("Body").Where(m => m.ID == newRec.ID).FirstOrDefault();
                    //удаляем запись в подчиненной таблице БД
                    db.RecordBodies.Remove(temp.Body);    
                    //удаляем запись основной таблицы БД                      
                    db.Records.Remove(temp);                                                         
                    db.SaveChanges();
                }
                //если поле автора пустое (если администратор редактирует запись другого пользователя не меняем автора)
                if (newRec.Author == null)                                                          
                {
                    //в поле автора записываем имя текущего пользователя
                    newRec.Author = User.Identity.Name;                                             
                }
                //заполняем поле даты последнего редактирования текущей датой
                newRec.Date = DateTime.Now;                                                         
                db.Records.Add(newRec);                                             
                db.SaveChanges();
                return RedirectToAction("Index"); 
            }
            else                                //если поля формы заполненны не корретно
            {
                return RedirectToAction("Add", "Home");
            }
        }

        //??????????????????????????????????????возможно этот метод не нужен если заменить его на просто добавление записи????????????????????????????????
        public ActionResult Edit(int id)
        {
            return View("Add", db.Records.Include("Body").Where(m => m.ID == id).First());
        }

        public ActionResult Delete(int id)
        {
            // удаление записей производим посредством аджакс для увеличения скорости работв приложения
            if (Request.IsAjaxRequest())
            {
                //если пользователь в роли администратора
                if (User.IsInRole("Admin"))
                {
                    // если запись уже удалена пользователем либо если автор записи является администратор
                    if (db.Records.Where(m => m.ID == id).Select(m => m).First().RemovedByUser == true || db.Records.Where(m => m.ID == id).Select(m => m).First().Author == User.Identity.Name)
                    {
                        Record deletedRecord = db.Records.Include("Body").Where(m => m.ID == id).Select(m => m).First();
                        // удаляем запись из подчиненной таблицы БД
                        db.RecordBodies.Remove(deletedRecord.Body);
                        //удаляем запись из основной таблицы БД
                        db.Records.Remove(deletedRecord);
                        db.SaveChanges();
                        return View("ListAjax", db.Records.Include("Body").ToList());
                    }
                    else    //если запись не удалена пользователем
                    {
                        //устанавливаем флаг об удалении записи администратором для отправки запроса пользователю на подтверждение удаления
                        db.Records.Where(m => m.ID == id).First().RemovedByAdmin = true;
                        db.SaveChanges();
                        return View("ListAjax", db.Records.Include("Body").ToList());
                    }
                }
                    //если пользователь не администратор
                else
                {
                    //если запись уже имеет флаг удаления администратором
                    if (db.Records.Where(m => m.ID == id).Select(m => m).First().RemovedByAdmin == true)
                    {
                        Record deletedRecord = db.Records.Include("Body").Where(m => m.ID == id).Select(m => m).First();
                        //удаляем запись в подчиненной таблице БД
                        db.RecordBodies.Remove(deletedRecord.Body);
                        //удаляем запись из основной таблицы БД
                        db.Records.Remove(deletedRecord);
                        db.SaveChanges();
                        return View("ListAjax", db.Records.Include("Body").Where(m=>m.Author == User.Identity.Name).ToList());
                    }
                    else    //если запиьс еще не удалена администратором
                    {
                        //устанавливаем флаг об удалении записи пользователем (отображается на странице администратора)
                        db.Records.Where(m => m.ID == id).FirstOrDefault().RemovedByUser = true;
                        db.SaveChanges();
                        return View("ListAjax", db.Records.Include("Body").Where(m => m.Author == User.Identity.Name && m.RemovedByUser == false).Select(m => m).ToList());
                    }
                }
            }
            else
            {
                db.Records.Remove(db.Records.Where(m => m.ID == id).Select(m => m).First());
                db.SaveChanges();
                return RedirectToAction("Index");
            } 
        }

        //метод восстановления записи удаленной пользователем 
        public ActionResult Restore (int id)
        {
            if (Request.IsAjaxRequest())
            {
                if(User.IsInRole("Admin"))
                {
                    db.Records.Where(m => m.ID == id).First().RemovedByUser = false;
                }
                else
                {
                    db.Records.Where(m => m.ID == id).First().RemovedByAdmin = false;
                }
                db.SaveChanges();
                return View("ListAjax", db.Records.Include("Body").ToList());
            }
            else return RedirectToAction("Index");
        }
    }
}