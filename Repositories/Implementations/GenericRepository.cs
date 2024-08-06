using DataFlowRRHH.Models;
using DataFlowRRHH.Repositories.Contracts;
using DataFlowRRHH.Response;
using Microsoft.EntityFrameworkCore;

namespace DataFlowRRHH.Repositories.Implementations
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly BdbioAdminSqlContext Context;
        public readonly DbSet<T> Entity;

        public GenericRepository(BdbioAdminSqlContext Context)
        {
            this.Context = Context;
            Entity = this.Context.Set<T>();
        }

        public virtual async Task<ActionResponse<T>> AddAsync(T entity)
        {
            Context.Add(entity);
            try
            {
                await Context.SaveChangesAsync();
                return new ActionResponse<T>
                {
                    WasSuccess = true,
                    Result = entity
                };
            }
            catch (DbUpdateException)
            {
                return GenericRepository<T>.DbUpdateExceptionActionResponse();

            }
            catch (Exception ex) 
            {
                return ExceptionActionResponse(ex);
            }




           
        }

        public virtual async Task<ActionResponse<T>> UpdateAsync(T entity)
        {
            Context.Update(Entity);
            try
            {
                await Context.SaveChangesAsync();
                return new ActionResponse<T>
                {
                    WasSuccess = true,
                    Result = entity
                };
            }
            catch (DbUpdateException)
            {
                return GenericRepository<T>.DbUpdateExceptionActionResponse();
            }
            catch (Exception ex) 
            {
                return ExceptionActionResponse(ex);
            }
        }

        public virtual async Task<ActionResponse<T>> DeleteAsync(int id)
        {
            var row = await Entity.FindAsync(id);
            if (row is null) 
            {
                return new ActionResponse<T>
                {
                    WasSuccess = false,
                    Message = "Regsitro no encontrado"
                };
            }
            try
            {
                Entity.Remove(row);
                await Context.SaveChangesAsync();
                return new ActionResponse<T>
                {
                    WasSuccess = true,
                };
            }
            catch
            {
                return new ActionResponse<T>
                {
                    WasSuccess = false,
                    Message = "No se puede Borrar, porque tuiene registros relacionados"
                };
            }
        }
        public virtual async Task<ActionResponse<T>> GetByIdAsync(int id)
        {
            var row = await Entity.FindAsync(id);
            if (row is null) 
            {
                return new ActionResponse<T>
                {
                    WasSuccess = false,
                    Message = "Registro no encontrado"
                };
            }
            return new ActionResponse<T>
            {
                WasSuccess=true,
                Result = row
            };
        }

        public IEnumerable<T> GetAllAsync()
        {
            var lista = Entity.ToList();
            return lista;
        }

        private static ActionResponse<T> DbUpdateExceptionActionResponse() 
        {
            return new ActionResponse<T>
            {
                WasSuccess = false,
                Message = "Ya existe un registro que esta intentando crear."
            };
        }

        private static ActionResponse<T> ExceptionActionResponse(Exception ex) 
        {
            return new ActionResponse<T>
            {
                WasSuccess=false,
                Message = ex.Message,
            };
        }

    }
}
