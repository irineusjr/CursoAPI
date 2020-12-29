using ApiCatalogo.Context;
using ApiCatalogo.DTOs;
using ApiCatalogo.Entities;
using ApiCatalogo.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiCatalogo.Controllers
{
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Produces("application/json")]
    [Route("api/[Controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        //private readonly CatalogoDBContext _context;
        private readonly IUnitOfWork _uof;
        private readonly IMapper _mapper;
        public CategoriasController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            //_context = contexto;
            _uof = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Obter todas as Categorias
        /// </summary>
        /// <returns>Objetos Categorias</returns>
        [HttpGet]
        //[ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetAsync()
        {
            var categorias = await _uof.CategoriaRepository.Get().ToListAsync();
            var categoriasDto = _mapper.Map<List<CategoriaDTO>>(categorias);
            return categoriasDto;
        }

        /// <summary>
        /// Recupera todas as categorias e seus respectivos produtos
        /// </summary>
        /// <returns>Objetos Categorias e Produtos</returns>
        [HttpGet("produtos")]
        //[ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetCategoriasProdutosAsync()
        {
            //return await _context.Categorias.Include(x => x.Produtos).ToListAsync();
            var categorias = await _uof.CategoriaRepository.GetCategoriasProdutos();
            var categoriasDto = _mapper.Map<List<CategoriaDTO>>(categorias);
            return categoriasDto;
        }

        /// <summary>
        /// Obter Categoria pelo id
        /// </summary>
        /// <param name="id">Id da Categoria</param>
        /// <returns>Dados da categoria especificada</returns>
        [HttpGet("{id}", Name = "ObterCategoria")]
        //[ProducesResponseType(typeof(Categoria),StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<CategoriaDTO>> GetByIdAsync(int id)
        {
            //var categoria = await _context.Categorias.AsNoTracking().FirstOrDefaultAsync(c => c.CategoriaId == id);
            var categoria = await _uof.CategoriaRepository.GetById(c => c.CategoriaId == id);
            if (categoria == null)
            {
                return NotFound();
            }
            return _mapper.Map<CategoriaDTO>(categoria);
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
        /// <param name="categoriaDto">Objeto Categoria</param>
        /// <returns>O objeto Categoria incluída</returns>
        /// <remarks>Retorna a Categoria inserida</remarks>
        [HttpPost]
        //[ProducesResponseType(typeof(Categoria), StatusCodes.Status201Created)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public async Task<ActionResult> PostAsync([FromBody]CategoriaDTO categoriaDto)
        {
            var categoria = _mapper.Map<Categoria>(categoriaDto);
            _uof.CategoriaRepository.Add(categoria);
            await _uof.Commit();

            return new CreatedAtRouteResult("ObterCategoria",
                new { id = categoria.CategoriaId }, categoriaDto);

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
        /// <param name="categoriaDto">objeto categoria</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        //[ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        public async Task<ActionResult> PutAsync(int id, [FromBody]CategoriaDTO categoriaDto)
        {
            if (id != categoriaDto.CategoriaId)
            {
                return BadRequest();
            }
            var categoria = _mapper.Map<Categoria>(categoriaDto);
            _uof.CategoriaRepository.Update(categoria);
            await _uof.Commit();
            return Ok();

        }
        /// <summary>
        /// Remove uma categoria da base de dados
        /// </summary>
        /// <param name="id">id da categoria</param>
        /// <returns>Retorna o objeto categoria excluído</returns>
        [HttpDelete("{id}")]
        //[ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
        public async Task<ActionResult<CategoriaDTO>> DeleteAsync(int id)
        {
            //var produto = _context.Produtos.FirstOrDefault(p => p.ProdutoId == id);
            var categoria = await _uof.CategoriaRepository.GetById(c => c.CategoriaId == id);
            if (categoria == null)
            {
                return NotFound();
            }
            var categoriaDto = _mapper.Map<CategoriaDTO>(categoria);
            _uof.CategoriaRepository.Delete(categoria);
            await _uof.Commit();
            return categoriaDto;
        }
    }
}
