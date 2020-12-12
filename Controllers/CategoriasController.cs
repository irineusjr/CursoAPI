using ApiCatalogo.Context;
using ApiCatalogo.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCatalogo.Controllers
{
    [Produces("application/json")]
    [Route("api/[Controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly CatalogoDBContext _context;
        public CategoriasController(CatalogoDBContext contexto)
        {
            _context = contexto;
        }

        /// <summary>
        /// Obter todas as Categorias
        /// </summary>
        /// <returns>Objetos Categorias</returns>
        [HttpGet]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<IEnumerable<Categoria>>> GetAsync()
        {
            return await _context.Categorias.AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// Recupera todas as categorias e seus respectivos produtos
        /// </summary>
        /// <returns>Objetos Categorias e Produtos</returns>
        [HttpGet("produtos")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<IEnumerable<Categoria>>> GetCategoriasProdutosAsync()
        {
            return await _context.Categorias.Include(x => x.Produtos).ToListAsync();
        }

        /// <summary>
        /// Obter Categoria pelo id
        /// </summary>
        /// <param name="id">Id da Categoria</param>
        /// <returns>Dados da categoria especificada</returns>
        [HttpGet("{id}", Name = "ObterCategoria")]
        //[ProducesResponseType(typeof(Categoria),StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<Categoria>> GetByIdAsync(int id)
        {
            var categoria = await _context.Categorias.AsNoTracking().FirstOrDefaultAsync(c => c.CategoriaId == id);
            if (categoria == null)
            {
                return NotFound();
            }
            return categoria;
        }

        /// <summary>
        /// Inclui uma nova categoria
        /// </summary>
        /// <remarks>
        /// Exemplo de request:
        ///
        ///         POST api/categorias
        ///         {
        ///            "categoriaId": 1,
        ///            "nome": "categoria1",
        ///            "imagemUrl": "http://teste.net/1.jpg"
        ///         }
        /// </remarks> 
        /// <param name="categoria">Objeto Categoria</param>
        /// <returns>O objeto Categoria incluída</returns>
        /// <remarks>Retorna a Categoria inserida</remarks>
        [HttpPost]
        //[ProducesResponseType(typeof(Categoria), StatusCodes.Status201Created)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public async Task<ActionResult> PostAsync([FromBody]Categoria categoria)
        {
            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();

            return new CreatedAtRouteResult("ObterCategoria",
                new { id = categoria.CategoriaId }, categoria);

        }
        /// <summary>
        /// Atualiza uma categoria, informando o id correspondente
        /// </summary>
        /// <remarks>
        /// Exemplo de request:
        ///
        ///         PUT api/categorias
        ///         {
        ///            "categoriaId": 7,
        ///            "nome": "Cama, mesa e banho",
        ///            "imagemUrl": "http://www.images.com/img4.jpg",
        ///         }
        /// </remarks>
        /// <param name="id">Id da Categoria</param>
        /// <param name="categoria">objeto categoria</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        public async Task<ActionResult> PutAsync(int id, [FromBody]Categoria categoria)
        {
            if (id != categoria.CategoriaId)
            {
                return BadRequest();
            }
            _context.Entry(categoria).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok();

        }
        /// <summary>
        /// Remove uma categoria da base de dados
        /// </summary>
        /// <param name="id">id da categoria</param>
        /// <returns>Retorna o objeto categoria excluído</returns>
        [HttpDelete("{id}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
        public async Task<ActionResult<Categoria>> DeleteAsync(int id)
        {
            //var produto = _context.Produtos.FirstOrDefault(p => p.ProdutoId == id);
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null)
            {
                return NotFound();
            }
            _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync();
            return categoria;
        }
    }
}
