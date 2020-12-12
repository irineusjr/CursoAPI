using ApiCatalogo.Context;
using ApiCatalogo.Entities;
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
    public class ProdutosController : ControllerBase
    {
        private readonly CatalogoDBContext _context;
        public ProdutosController(CatalogoDBContext contexto)
        {
            _context = contexto;
        }

        /// <summary>
        /// Obtem todos os produtos
        /// </summary>
        /// <returns>Retorna todos os produtos</returns>
        [HttpGet]
        public ActionResult<IEnumerable<Produto>> Get()
        {
            return _context.Produtos.AsNoTracking().ToList();
        }
        
        /// <summary>
        /// Obtem um produto pelo id
        /// </summary>
        /// <param name="id">Id do produto</param>
        /// <returns>Objeto produto</returns>
        [HttpGet("{id}", Name = "ObterProduto")]
        public ActionResult<Produto> GetById(int id)
        {
            var produto = _context.Produtos.Find(id);
            if (produto == null)
            {
                return NotFound();
            }
            return produto;
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
        /// <param name="produto">Objeto produto</param>
        /// <returns>Objeto produto cadastrado</returns>
        [HttpPost]
        public ActionResult Post([FromBody]Produto produto)
        {
            _context.Produtos.Add(produto);
             _context.SaveChanges();

            return new CreatedAtRouteResult("ObterProduto",
                new { id = produto.ProdutoId }, produto);

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
        /// <param name="produto">objeto produto</param>
        /// <returns>Status do objeto modificado</returns>
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody]Produto produto)
        {
            if (id != produto.ProdutoId)
            {
                return BadRequest();
            }
            _context.Entry(produto).State = EntityState.Modified;
             _context.SaveChangesAsync();
            return Ok();

        }
        /// <summary>
        /// Exclui um produto pelo id
        /// </summary>
        /// <param name="id">Id do produto</param>
        /// <returns>Objeto produto excluído</returns>
        [HttpDelete("{id}")]
        public ActionResult<Produto> Delete(int id)
        {
            //var produto = _context.Produtos.FirstOrDefault(p => p.ProdutoId == id);
            var produto = _context.Produtos.Find(id);
            if (produto == null)
            {
                return NotFound();
            }
            _context.Produtos.Remove(produto);
            _context.SaveChanges();
            return produto;
        }
    }
}
