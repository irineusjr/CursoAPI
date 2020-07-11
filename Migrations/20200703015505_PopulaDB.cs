using Microsoft.EntityFrameworkCore.Migrations;

namespace ApiCatalogo.Migrations
{
    public partial class PopulaDB : Migration
    {
        protected override void Up(MigrationBuilder mb)
        {
            mb.Sql("insert into Categorias(Nome,ImagemUrl) Values ('Bebidas'," +
                "'http://www.images.com/img1.jpg')");
            mb.Sql("insert into Categorias(Nome,ImagemUrl) Values ('Alimentos'," +
                "'http://www.images.com/img2.jpg')");
            mb.Sql("insert into Categorias(Nome,ImagemUrl) Values ('Produtos de Limpeza'," +
                "'http://www.images.com/img3.jpg')");

            mb.Sql("insert into Produtos(Nome,Descricao,Preco,ImagemUrl,Estoque,DataCadastro,CategoriaId) Values" +
                " ('Coca-Cola','Refrigerante de Cola',5.45,'http://www.images.com/prod1.jpg',500,Now()," +
                "(Select CategoriaId from Categorias where Nome = 'Bebidas'))");
            mb.Sql("insert into Produtos(Nome,Descricao,Preco,ImagemUrl,Estoque,DataCadastro,CategoriaId) Values" +
                " ('Guaraná','Refrigerante de Guaraná',4.47,'http://www.images.com/prod2.jpg',1000,Now()," +
                "(Select CategoriaId from Categorias where Nome = 'Bebidas'))");

            mb.Sql("insert into Produtos(Nome,Descricao,Preco,ImagemUrl,Estoque,DataCadastro,CategoriaId) Values" +
                " ('Arroz','Arroz',1.20,'http://www.images.com/prod3.jpg',55,Now()," +
                "(Select CategoriaId from Categorias where Nome = 'Alimentos'))");
            mb.Sql("insert into Produtos(Nome,Descricao,Preco,ImagemUrl,Estoque,DataCadastro,CategoriaId) Values" +
                " ('Detergente','Detergente',2.15,'http://www.images.com/prod4.jpg',76,Now()," +
                "(Select CategoriaId from Categorias where Nome = 'Produtos de Limpeza'))");
        }

        protected override void Down(MigrationBuilder mb)
        {
            mb.Sql("Delete from Categorias");
            mb.Sql("Delete from Produtos");

        }
    }
}
