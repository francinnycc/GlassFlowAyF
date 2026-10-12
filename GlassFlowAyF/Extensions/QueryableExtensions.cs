using GlassFlowAyF.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace GlassFlowAyF.Extensions
{
    public static class QueryableExtensions
    {
        public static async Task<PaginadoViewModel<T>>
            PaginarAsync<T>(
                this IQueryable<T> consulta,
                int pagina = 1,
                int registrosPorPagina = 10)
        {
            if (pagina < 1)
                pagina = 1;

            if (registrosPorPagina < 1)
                registrosPorPagina = 10;

            var totalRegistros =
                await consulta.CountAsync();

            var registros =
                await consulta
                    .Skip(
                        (pagina - 1) *
                        registrosPorPagina)
                    .Take(registrosPorPagina)
                    .ToListAsync();

            return new PaginadoViewModel<T>
            {
                Registros = registros,

                Paginacion =
                    new PaginacionViewModel
                    {
                        PaginaActual = pagina,

                        RegistrosPorPagina =
                            registrosPorPagina,

                        TotalRegistros =
                            totalRegistros
                    }
            };
        }
    }
}