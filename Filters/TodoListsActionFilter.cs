using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TodoAPI.Models;
using TodoAPI.Services;

namespace TodoAPI.Filters
{
    public class TodoListsActionFilter : ActionFilterAttribute, IActionFilter
    {
        private readonly TodoContext _context;
        private readonly IAuthService _authService;

        public TodoListsActionFilter(TodoContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        public override async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next) {
            
            var id = Int32.Parse(context.ActionArguments["id"].ToString());

            var user = _authService.GetUserFromIdentity(context.HttpContext.User.Identity);
            var todoList = _context.TodoLists
                .Include(tl => tl.TodoItems)
                .SingleOrDefault(tl => tl.Id == id);

            if (todoList == null) {
                context.Result = new JsonResult(
                    new ResponseModel {
                        Success = false,
                        Error = new ErrorModel {
                            Message = "This list does not exist."
                        }
                    }
                ) {
                    StatusCode = 404
                };
            }

            var userProject = _context.UserProjects
                .SingleOrDefault(up => up.ProjectId == todoList.ProjectId && up.UserId == user.Id);

            if (userProject == null) {
                context.Result = new JsonResult(
                    new ResponseModel {
                        Success = false,
                        Error = new ErrorModel {
                            Message = "The project doesn't exist or the current user doesn't have permission to access this project."
                        }
                    }
                ) {
                    StatusCode = 400
                };
            } else {
                context.HttpContext.Items["todoList"] = todoList;
                await next();
            }
        }
    }
}