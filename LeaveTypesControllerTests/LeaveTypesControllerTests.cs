using eOdsustva.SoftverskoInzenjerstvo.Controllers;
using eOdsustva.SoftverskoInzenjerstvo.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eOdsustva.xUnitTests
{
    public class LeaveTypesControllerTests
    {
        
        private static ApplicationDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        private static async Task SeedLeaveTypesAsync(ApplicationDbContext context)
        {
            context.LeaveTypes.AddRange(
                new LeaveType { Id = 1, Name = "Annual", NumberOfDays = 20 },
                new LeaveType { Id = 2, Name = "Sick", NumberOfDays = 10 }
            );
            await context.SaveChangesAsync();
        }


        [Fact]
        public async Task Index_ReturnsViewResult()
        {
            //Arrange
            var context = CreateInMemoryContext();
            var controller = new LeaveTypesController(context);

            // Act
            var result = await controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Index_ReturnsViewWithListOfLeaveTypes()
        {
            // Arrange
            var context = CreateInMemoryContext();
            await SeedLeaveTypesAsync(context);
            var controller = new LeaveTypesController(context);

            // Act
            var result = await controller.Index();

            // Assert
            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<List<LeaveType>>(view.Model);
            Assert.Equal(2, model.Count);
        }

        [Fact]
        public async Task Index_WhenNoLeaveTypesExist_ReturnsEmptyList()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var controller = new LeaveTypesController(context);

            // Act
            var result = await controller.Index();

            
            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<List<LeaveType>>(view.Model);
            Assert.Empty(model);
        }

        
        [Fact]
        public void Create_Get_ReturnsViewResult()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var controller = new LeaveTypesController(context);

            var result = controller.Create();

            Assert.IsType<ViewResult>(result);
        }


        
        [Fact]
        public async Task Create_Post_ValidModel_AddsLeaveTypeToDatabase_AndRedirectsToIndex()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var controller = new LeaveTypesController(context);

            var newItem = new LeaveType { Name = "Unpaid", NumberOfDays = 5 };

            var result = await controller.Create(newItem);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(LeaveTypesController.Index), redirect.ActionName);

            var all = await context.LeaveTypes.ToListAsync();
            Assert.Single(all);
            Assert.Equal("Unpaid", all[0].Name);
            Assert.Equal(5, all[0].NumberOfDays);
        }

        [Fact]
        public async Task Create_Post_InvalidModel_ReturnsViewWithModel_AndDoesNotAddToDatabase()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var controller = new LeaveTypesController(context);

            controller.ModelState.AddModelError("Name", "Required");

            var newItem = new LeaveType { Name = "", NumberOfDays = 5 };

            var result = await controller.Create(newItem);

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<LeaveType>(view.Model);

            Assert.Equal("", model.Name);

            var all = await context.LeaveTypes.ToListAsync();
            Assert.Empty(all);
        }

        
        [Fact]
        public async Task Details_NullId_ReturnsNotFound()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var controller = new LeaveTypesController(context);

            var result = await controller.Details(null);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var context = CreateInMemoryContext();
            await SeedLeaveTypesAsync(context);
            var controller = new LeaveTypesController(context);

            var result = await controller.Details(999);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_ExistingId_ReturnsViewWithLeaveType()
        {
            // Arrange
            var context = CreateInMemoryContext();
            await SeedLeaveTypesAsync(context);
            var controller = new LeaveTypesController(context);

            var result = await controller.Details(1);

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<LeaveType>(view.Model);
            Assert.Equal(1, model.Id);
            Assert.Equal("Annual", model.Name);
        }


        [Fact]
        public async Task Edit_Get_NullId_ReturnsNotFound()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var controller = new LeaveTypesController(context);

            var result = await controller.Edit(null);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_Get_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var context = CreateInMemoryContext();
            await SeedLeaveTypesAsync(context);
            var controller = new LeaveTypesController(context);

            var result = await controller.Edit(999);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_Get_ExistingId_ReturnsViewWithLeaveType()
        {
            // Arrange
            var context = CreateInMemoryContext();
            await SeedLeaveTypesAsync(context);
            var controller = new LeaveTypesController(context);

            var result = await controller.Edit(2);

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<LeaveType>(view.Model);
            Assert.Equal(2, model.Id);
            Assert.Equal("Sick", model.Name);
        }


        [Fact]
        public async Task Edit_Post_IdMismatch_ReturnsNotFound()
        {
            // Arrange
            var context = CreateInMemoryContext();
            await SeedLeaveTypesAsync(context);
            var controller = new LeaveTypesController(context);

            var edited = new LeaveType { Id = 2, Name = "Sick", NumberOfDays = 15 };

            var result = await controller.Edit(999, edited);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_Post_InvalidModel_ReturnsViewWithModel_AndDoesNotUpdateDatabase()
        {
            // Arrange
            var context = CreateInMemoryContext();
            await SeedLeaveTypesAsync(context);
            var controller = new LeaveTypesController(context);

            controller.ModelState.AddModelError("Name", "Required");

            var edited = new LeaveType { Id = 1, Name = "", NumberOfDays = 99 };

            var result = await controller.Edit(1, edited);

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<LeaveType>(view.Model);
            Assert.Equal(1, model.Id);

            var fromDb = await context.LeaveTypes.FindAsync(1);
            Assert.NotNull(fromDb);
            Assert.Equal("Annual", fromDb!.Name);
            Assert.Equal(20, fromDb.NumberOfDays);
        }

        [Fact]
        public async Task Edit_Post_ValidModel_UpdatesDatabase_AndRedirectsToIndex()
        {
            // Arrange
            var context = CreateInMemoryContext();
            await SeedLeaveTypesAsync(context);

            context.ChangeTracker.Clear();

            var controller = new LeaveTypesController(context);
            var edited = new LeaveType { Id = 1, Name = "Annual Updated", NumberOfDays = 25 };

            var result = await controller.Edit(1, edited);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(LeaveTypesController.Index), redirect.ActionName);

            var fromDb = await context.LeaveTypes.FindAsync(1);
            Assert.NotNull(fromDb);
            Assert.Equal("Annual Updated", fromDb!.Name);
            Assert.Equal(25, fromDb.NumberOfDays);
        }


        [Fact]
        public async Task Delete_Get_NullId_ReturnsNotFound()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var controller = new LeaveTypesController(context);

            var result = await controller.Delete(null);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_Get_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var context = CreateInMemoryContext();
            await SeedLeaveTypesAsync(context);
            var controller = new LeaveTypesController(context);

            var result = await controller.Delete(999);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_Get_ExistingId_ReturnsViewWithLeaveType()
        {
            // Arrange
            var context = CreateInMemoryContext();
            await SeedLeaveTypesAsync(context);
            var controller = new LeaveTypesController(context);

            var result = await controller.Delete(2);

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<LeaveType>(view.Model);
            Assert.Equal(2, model.Id);
            Assert.Equal("Sick", model.Name);
        }


        [Fact]
        public async Task DeleteConfirmed_ExistingId_RemovesLeaveTypeFromDatabase_AndRedirectsToIndex()
        {
            // Arrange
            var context = CreateInMemoryContext();
            await SeedLeaveTypesAsync(context);
            var controller = new LeaveTypesController(context);

            var result = await controller.DeleteConfirmed(1);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(LeaveTypesController.Index), redirect.ActionName);

            var fromDb = await context.LeaveTypes.FindAsync(1);
            Assert.Null(fromDb);

            var all = await context.LeaveTypes.ToListAsync();
            Assert.Single(all); 
        }

        [Fact]
        public async Task DeleteConfirmed_NonExistingId_DoesNotThrow_AndRedirectsToIndex()
        {
            // Arrange
            var context = CreateInMemoryContext();
            await SeedLeaveTypesAsync(context);
            var controller = new LeaveTypesController(context);

            // Act
            var result = await controller.DeleteConfirmed(999);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(LeaveTypesController.Index), redirect.ActionName);

            var all = await context.LeaveTypes.ToListAsync();
            Assert.Equal(2, all.Count); 
        } 
    }
}
