using Microsoft.EntityFrameworkCore;
using WhileImHere.Controllers;
using WhileImHere.Data;
using WhileImHere.Models;
using Microsoft.AspNetCore.Mvc;
using Task = WhileImHere.Models.Task;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WhileImHereTests
{
    [TestClass]
    public class TasksControllerTests
    {
        #region "Test Initialize"
        private ApplicationDbContext context;
        TasksController controller;

        [TestInitialize]
        public void TestInitialize()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            context = new ApplicationDbContext(options);

            //seed the db before passing it to the controller
            var location = new Location { LocationID = 367, LocationName = "Some Location", LocationRadius = "20", LocationPriority = 1};
            context.Add(location);

            for (var i = 100; i < 111; i++)
            {
                var task = new Task { TaskID = i, Name = "Task " + i.ToString(), LocationID = 367, TaskRadius = i + 10, TaskStreetAddress = "123" + i  };
                context.Add(task);
            }
            context.SaveChanges();

            controller = new TasksController(context);
        }
        #endregion
        #region "Index"
        [TestMethod]
        public void IndexLoadsView()
        {
            //act
            var result = (ViewResult)controller.Index().Result;

            //assert
            Assert.AreEqual("Index", result.ViewName);
        }
        [TestMethod]
        public void IndexLoadsTasks()
        {
            //act
            var result = (ViewResult)controller.Index().Result;
            List<Task> model = (List<Task>)result.Model;
            //assert
            CollectionAssert.AreEqual(context.Tasks.ToList(), model);
        }
        #endregion
        #region "Details"
        [TestMethod]
        public void DetailsNoIdLoads404()
        {
            //act 
            var result = (ViewResult)controller.Details(null).Result;

            //assert
            Assert.AreEqual("404", result.ViewName);
        }

        [TestMethod]
        public void DetailsNoTasksTableLoads404()
        {
            //arrange
            context.Tasks = null;

            //act 
            var result = (ViewResult)controller.Details(null).Result;

            //assert
            Assert.AreEqual("404", result.ViewName);
        }

        [TestMethod]
        public void DetailsInvalidIdLoads404()
        {
            //act 
            var result = (ViewResult)controller.Details(15).Result;

            //assert
            Assert.AreEqual("404", result.ViewName);
        }

        [TestMethod]
        public void DetialsValidIdLoadsView()
        {
            //act 
            var result = (ViewResult)controller.Details(102).Result;

            //assert
            Assert.AreEqual("Details", result.ViewName);
        }

        [TestMethod]
        public void DetialsValidIdLoadsTask()
        {
            //act 
            var result = (ViewResult)controller.Details(102).Result;

            //assert
            Assert.AreEqual(context.Tasks.Find(102), result.Model);
        }
        #endregion
        #region "Create"
        [TestMethod]
        public void CreateLoadsView()
        {
            //act
            var result = (ViewResult)controller.Create();

            //assert
            Assert.AreEqual("Create", result.ViewName);
        }
        #endregion
        #region "Create POST"
        [TestMethod]
        public void CreatePostModelStateInvalidLoadsView()
        {
            // arrange
            var task = new Task { TaskID = 1, Name = "Task 1", TaskRadius = 51 };
            controller.ModelState.AddModelError("Name", "Name is required");
            controller.ModelState.AddModelError("TaskStreetAddress", "Task street address is required");

            // act
            var result = (ViewResult)controller.Create(task).Result;

            // assert
            Assert.AreEqual("Create", result.ViewName);
        }
        [TestMethod]
        public void CreatePostModelStateValidLoadsView()
        {
            var task = new Task { TaskID = 1, Name = "Task 1", LocationID = 367, TaskRadius = 51, TaskStreetAddress = "111" };
            //act 
            var result = (RedirectToActionResult)controller.Create(task).Result;
            Assert.AreEqual("Index", result.ActionName);
        }

        #endregion
        #region "Edit"
        [TestMethod]
        public void EditNoId()
        {
            //act 
            var result = (ViewResult)controller.Edit(null).Result;

            //assert
            Assert.AreEqual("404", result.ViewName);
        }
        [TestMethod]
        public void EditNoTasksTableLoads404()
        {
            //arrange
            context.Tasks = null;

            //act 
            var result = (ViewResult)controller.Edit(104).Result;

            //assert
            Assert.AreEqual("404", result.ViewName);
        }

        [TestMethod]
        public void EditInvalidIdLoads404()
        {
            //act 
            var result = (ViewResult)controller.Edit(15).Result;

            //assert
            Assert.AreEqual("404", result.ViewName);
        }

        [TestMethod]
        public void EditValidIdLoadsView()
        {
            //act 
            var result = (ViewResult)controller.Edit(102).Result;

            //assert
            Assert.AreEqual("Edit", result.ViewName);
        }

        [TestMethod]
        public void EditValidIdLoadsTask()
        {
            //act 
            var result = (ViewResult)controller.Details(102).Result;

            //assert
            Assert.AreEqual(context.Tasks.Find(102), result.Model);
        }
        #endregion
        #region "Edit POST"

        [TestMethod]
        public void EditPostInvalidIdLoads404() 
        {
            var task = new Task { TaskID = 2, Name = "Task 1", LocationID = 367, TaskRadius = 51, TaskStreetAddress = "111" };
            //act 
            var result = (ViewResult)controller.Edit(1, task).Result;
            //assert
            Assert.AreEqual("404", result.ViewName);

        }
        [TestMethod]
        public void EditPostNoTasksTableLoads404()
        {
            //arrange
            context.Tasks = null;

            //act 
            var result = (ViewResult)controller.Edit(1).Result;

            //assert
            Assert.AreEqual("404", result.ViewName);
        }
        [TestMethod]
        public void EditPostModelStateInvalidLoadsView()
        {
            // arrange
            var task = new Task { TaskID = 1, Name = "Task 1", TaskRadius = 51 };
            controller.ModelState.AddModelError("Name", "Name is required");
            controller.ModelState.AddModelError("TaskStreetAddress", "Task street address is required");

            // act
            var result = (ViewResult)controller.Edit(1, task).Result;

            // assert
            Assert.AreEqual("Edit", result.ViewName);
        }
        [TestMethod]
        public void EditPostModelStateValidLoadsView()
        {
            //act 
            var result = (RedirectToActionResult)controller.Edit(102, context.Tasks.Find(102)).Result;
            //assert
            Assert.AreEqual("Index", result.ActionName);
            
        }

        #endregion
        #region "Delete"
        [TestMethod]
        public void DeleteNoId()
        {
            //act 
            var result = (ViewResult)controller.Delete(null).Result;

            //assert
            Assert.AreEqual("404", result.ViewName);
        }
        [TestMethod]
        public void DeleteNoTasksTableLoads404()
        {
            //arrange
            context.Tasks = null;

            //act 
            var result = (ViewResult)controller.Delete(null).Result;

            //assert
            Assert.AreEqual("404", result.ViewName);
        }

        [TestMethod]
        public void DeleteInvalidIdLoads404()
        {
            //act 
            var result = (ViewResult)controller.Delete(15).Result;

            //assert
            Assert.AreEqual("404", result.ViewName);
        }

        [TestMethod]
        public void DeleteValidIdLoadsView()
        {
            //act 
            var result = (ViewResult)controller.Delete(102).Result;

            //assert
            Assert.AreEqual("Delete", result.ViewName);
        }

        [TestMethod]
        public void DeleteValidIdLoadsTask()
        {
            //act 
            var result = (ViewResult)controller.Delete(102).Result;

            //assert
            Assert.AreEqual(context.Tasks.Find(102), result.Model);
        }
        #endregion
        #region "Delete Confirmed"
        [TestMethod]
        public void DeleteConfirmedLoadsView()
        {
            //act
            var result = (RedirectToActionResult)controller.DeleteConfirmed(104).Result;

            //assert 
            Assert.AreEqual("Index", result.ActionName);
        }

        #endregion


    }
}