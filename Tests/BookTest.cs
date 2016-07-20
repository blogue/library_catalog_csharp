using Xunit;
using System.Collections.Generic;
using System;
using LibraryCatalog.Objects;
using System.Data;
using System.Data.SqlClient;

namespace LibraryCatalog.Tests
{
  public class LibraryCatalogTests : IDisposable
  {
    DateTime? testDate = new DateTime(1990, 09, 05);
    public LibraryCatalogTests()
    {
      DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=library_catalog_test;Integrated Security=SSPI;";
    }

    public void Dispose()
    {
      Book.DeleteAll();
    }
    [Fact]
    public void Book_DatabaseEmpty()
    {
      //Arrange, Act
      int result = Book.GetAll().Count;
      //Assert
      Assert.Equal(0, result);
    }
    [Fact]
    public void Book_Save_SavesBookToDatabase()
    {
      Book newBook = new Book("Cats", testDate, 2);

      newBook.Save();
      List<Book> expectedResult = new List<Book>{newBook};
      List<Book> actualResult = Book.GetAll();

      Assert.Equal(expectedResult, actualResult);
    }
    [Fact]
    public void Book_FindsBookInDatabase()
    {
      //Arrange
      Book expectedResult = new Book("Crime & Punishment", testDate, 3);
      expectedResult.Save();
      //Act
      Book result = Book.Find(expectedResult.GetId());
      //Assert
      Assert.Equal(expectedResult, result);
    }
    [Fact]
    public void Book_Delete_DeletesBookById()
    {
      Book firstBook = new Book("Cats", testDate, 2);
      firstBook.Save();
      Book secondBook = new Book("Crime & Punishment", testDate, 3);
      secondBook.Save();

      firstBook.Delete();

      List<Book> expectedResult = new List<Book>{secondBook};
      List<Book> actualResult = Book.GetAll();
      Assert.Equal(expectedResult, actualResult);
    }
    [Fact]
    public void Book_AddAuthors()
    {
      //Arrange
      Book newBook = new Book("Cats", testDate, 2);
      newBook.Save();
      Author newAuthor = new Author("Chad");
      newAuthor.Save();
      //Act
      newBook.AddAuthor(newAuthor.GetId());
      List<Author> result = newBook.GetAuthors();
      List<Author> expectedResult = new List<Author>{newAuthor};
      //Assert
      Assert.Equal(expectedResult, result);
    }
  }
}