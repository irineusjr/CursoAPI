using ApiCatalogo.DTOs;
using ApiCatalogo.Entities;
using ApiCatalogo.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCatalogo.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]  //retorna 401 se o usuario nao estiver autenticado
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Produces("application/json")]
    [Route("api/[Controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        //private readonly CatalogoDBContext _context;
        //public ProdutosController(CatalogoDBContext contexto)
        //{
        //    _context = contexto;
        //}

        private readonly IUnitOfWork _uof;
        private readonly IMapper _mapper;

        public ProdutosController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _uof = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtem todos os produtos
        /// </summary>
        /// <returns>Retorna todos os produtos</returns>
        [HttpGet]
        //[ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> Get()
        {
            var produtos = await _uof.ProdutoRepository.Get().ToListAsync();
            var produtosDto = _mapper.Map<List<ProdutoDTO>>(produtos);
            return produtosDto;
            //return await _uof.ProdutoRepository.Get().ToListAsync();
        }
        
        /// <summary>
        /// Obtem um produto pelo id
        /// </summary>
        /// <param name="id">Id do produto</param>
        /// <returns>Objeto produto</returns>
        [HttpGet("{id}", Name = "ObterProduto")]
        //[ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<ProdutoDTO>> GetById(int id)
        {
            var produto = await _uof.ProdutoRepository.GetById(p => p.ProdutoId == id);
            if (produto == null)
            {
                return NotFound();
            }

            var produtoDto = _mapper.Map<ProdutoDTO>(produto);
            return produtoDto;
        }

        /// <summary>
        /// Cadastra um novo produto
        /// </summary>
        /// <remarks>
        /// Exemplo de request:
        ///
        ///         POST api/produtos
        ///         {
        ///                "nome": "",
        ///                "descricao": "",
        ///                "preco": 0,
        ///                "imagemUrl": "http://www.images.com/prod31.jpg",
        ///                "estoque": 100,
        ///                "dataCadastro": "2020-09-12T19:22:00",
        ///                "categoriaId": 9
        ///         }
        /// </remarks>
        /// <param name="produtoDto">Objeto produto</param>
        /// <returns>Objeto produto cadastrado</returns>
        [HttpPost]
        //[ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public async Task<ActionResult> Post([FromBody]ProdutoDTO produtoDto)
        {
            var produto = _mapper.Map<Produto>(produtoDto);
            _uof.ProdutoRepository.Add(produto);
            await _uof.Commit();

            return new CreatedAtRouteResult("ObterProduto",
                new { id = produto.ProdutoId }, produtoDto);

        }
        /// <summary>
        /// Modifica um produto pelo id
        /// </summary>
        /// <remarks>
        /// Exemplo de request:
        ///
        ///         PUT api/produtos
        ///         {
        ///                "nome": "",
        ///                "descricao": "",
        ///                "preco": 0,
        ///                "imagemUrl": "http://www.images.com/prod31.jpg",
        ///                "estoque": 100,
        ///                "dataCadastro": "2020-09-12T19:22:00",
        ///                "categoriaId": 9
        ///         }
        /// </remarks>
        /// <param name="id">id do produto a ser modificado</param>
        /// <param name="produtoDto">objeto produto</param>
        /// <returns>Status do objeto modificado</returns>
        [HttpPut("{id}")]
        //[ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        public async Task<ActionResult> Put(int id, [FromBody]ProdutoDTO produtoDto)
        {
            if (id != produtoDto.ProdutoId)
            {
                return BadRequest();
            }
            var produto = _mapper.Map<Produto>(produtoDto);
            _uof.ProdutoRepository.Update(produto);
            await _uof.Commit();
            return Ok();
        }
        /// <summary>
        /// Exclui um produto pelo id
        /// </summary>
        /// <param name="id">Id do produto</param>
        /// <returns>Objeto produto excluído</returns>
        [HttpDelete("{id}")]
        //[ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
        public async Task<ActionResult<ProdutoDTO>> Delete(int id)
        {
            //var produto = _context.Produtos.FirstOrDefault(p => p.ProdutoId == id);
            //var produto = _context.Produtos.Find(id);
            var produto = await _uof.ProdutoRepository.GetById(p => p.ProdutoId == id);

            if (produto == null)
            {
                return NotFound();
            }

            var produtoDto = _mapper.Map<ProdutoDTO>(produto);

            //_context.Produtos.Remove(produto);
            //_context.SaveChanges();
            //return produto;
            _uof.ProdutoRepository.Delete(produto);
            await _uof.Commit();
            return produtoDto;
        }
    }
}
