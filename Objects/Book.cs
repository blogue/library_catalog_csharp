using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace LibraryCatalog.Objects
{
  public class Book
  {
    private int _id;
    private string _title;
    private DateTime? _datePublished;
    private int _genreId;

    public Book(string title, DateTime? datePublished, int genreId, int Id=0)
    {
      _title = title;
      _datePublished = datePublished;
      _genreId = genreId;
      _id = Id;
    }

    public int GetId()
    {
      return _id;
    }
    public string GetTitle()
    {
      return _title;
    }
    public DateTime? GetDatePublished()
    {
      return _datePublished;
    }
    public int GetGenreId()
    {
      return _genreId;
    }

    public static List<Book> GetAll()
    {
      List<Book> allBooks = new List<Book>{};

      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM books;", conn);
      rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        int bookId = rdr.GetInt32(0);
        string bookTitle = rdr.GetString(1);
        DateTime? bookPublishDate = rdr.GetDateTime(2);
        int bookGenre = rdr.GetInt32(3);
        Book newBook = new Book(bookTitle, bookPublishDate, bookGenre, bookId);
        allBooks.Add(newBook);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }

      return allBooks;
    }

    public void Save()
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO books (title, year_published, genre_id) OUTPUT INSERTED.id VALUES (@BookTitle, @YearPublished, @GenreId);", conn);

      SqlParameter titleParameter = new SqlParameter();
      titleParameter.ParameterName = "@BookTitle";
      titleParameter.Value = this.GetTitle();
      cmd.Parameters.Add(titleParameter);

      SqlParameter yearParameter = new SqlParameter();
      yearParameter.ParameterName = "@YearPublished";
      yearParameter.Value = this.GetDatePublished();
      cmd.Parameters.Add(yearParameter);

      SqlParameter genreParameter = new SqlParameter();
      genreParameter.ParameterName = "@GenreId";
      genreParameter.Value = this.GetGenreId();
      cmd.Parameters.Add(genreParameter);

      rdr=cmd.ExecuteReader();

      while(rdr.Read())
      {
        this._id = rdr.GetInt32(0);
      }
      if(rdr!=null) rdr.Close();
      if(conn!=null) conn.Close();
    }

    public static void DeleteAll()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("DELETE FROM books; DELETE FROM books_authors;", conn);
      cmd.ExecuteNonQuery();
    }

    public override bool Equals(System.Object otherBook)
    {
      if(!(otherBook is Book))
      {
        return false;
      }
      else
      {
        Book newBook = (Book) otherBook;
        bool bookIdEquality = _id == newBook.GetId();
        bool bookTitleEquality = _title == newBook.GetTitle();
        bool bookPublishDateEquality = _datePublished == newBook.GetDatePublished();
        bool bookGenreIdEquality = _genreId == newBook.GetGenreId();
        return (bookIdEquality && bookTitleEquality && bookPublishDateEquality && bookGenreIdEquality);
      }
    }

    public static Book Find(int bookId)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlDataReader rdr = null;

      SqlCommand cmd = new SqlCommand("SELECT * FROM books WHERE id = @BookId;", conn);

      SqlParameter bookIdParameter = new SqlParameter();
      bookIdParameter.ParameterName = "@BookId";
      bookIdParameter.Value = bookId;
      cmd.Parameters.Add(bookIdParameter);

      int foundBookId = 0;
      string foundBookTitle = null;
      DateTime? foundBookPublishedDate = null;
      int foundBookGenreId = 0;

      rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        foundBookId = rdr.GetInt32(0);
        foundBookTitle = rdr.GetString(1);
        foundBookPublishedDate = rdr.GetDateTime(2);
        foundBookGenreId = rdr.GetInt32(3);
      }
      Book foundBook = new Book(foundBookTitle, foundBookPublishedDate, foundBookGenreId, foundBookId);

      if(rdr!=null) rdr.Close();
      if(conn!=null) conn.Close();

      return foundBook;
    }

    public void Delete()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("DELETE FROM books WHERE id = @BookId;", conn);

      SqlParameter bookIdParameter = new SqlParameter();
      bookIdParameter.ParameterName = "@BookId";
      bookIdParameter.Value = this.GetId();
      cmd.Parameters.Add(bookIdParameter);

      cmd.ExecuteNonQuery();

      if(conn!=null) conn.Close();
    }

    public void AddAuthor(int authorId)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO books_authors (book_id, author_id) VALUES (@BookId, @AuthorId);", conn);

      SqlParameter bookIdParameter = new SqlParameter();
      bookIdParameter.ParameterName = "@BookId";
      bookIdParameter.Value = this.GetId();
      cmd.Parameters.Add(bookIdParameter);

      SqlParameter authorIdParameter = new SqlParameter();
      authorIdParameter.ParameterName = "@AuthorId";
      authorIdParameter.Value = authorId;
      cmd.Parameters.Add(authorIdParameter);

      cmd.ExecuteNonQuery();

      if(conn!=null) conn.Close();
    }

    public void DeleteAuthor(int authorId)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("DELETE FROM books_authors WHERE book_id = @BookId AND author_id = @AuthorId;", conn);

      SqlParameter bookIdParameter = new SqlParameter();
      bookIdParameter.ParameterName = "@BookId";
      bookIdParameter.Value = this.GetId();
      cmd.Parameters.Add(bookIdParameter);

      SqlParameter authorIdParameter = new SqlParameter();
      authorIdParameter.ParameterName = "@AuthorId";
      authorIdParameter.Value = authorId;
      cmd.Parameters.Add(authorIdParameter);

      cmd.ExecuteNonQuery();

      if(conn!=null) conn.Close();
    }

    public List<Author> GetAuthors()
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT authors.* FROM books JOIN books_authors ON (books_authors.book_id = books.id) JOIN authors ON (books_authors.author_id = authors.id) WHERE book_id = @BookId;", conn);

      SqlParameter bookIdParameter = new SqlParameter();
      bookIdParameter.ParameterName = "@BookId";
      bookIdParameter.Value = this.GetId();
      cmd.Parameters.Add(bookIdParameter);

      rdr=cmd.ExecuteReader();

      List<Author> foundAuthors = new List<Author>{};

      while(rdr.Read())
      {
        int foundId = rdr.GetInt32(0);
        string foundName = rdr.GetString(1);
        Author foundAuthor = new Author(foundName, foundId);
        foundAuthors.Add(foundAuthor);
      }

      if(rdr!=null) rdr.Close();
      if(conn!=null) conn.Close();

      return foundAuthors;
    }

    public string GetGenre()
    {
      int genreId = this.GetGenreId();
      Genre genre = Genre.Find(genreId);
      string currentGenre = genre.GetName();
      return currentGenre;
    }

    public void Update(string newTitle, DateTime? newPublicationDate, int newGenreId)
    {
      _title = newTitle;
      _datePublished = newPublicationDate;
      _genreId = newGenreId;

      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("UPDATE books SET title=@NewTitle WHERE id=@BookId; UPDATE books SET year_published=@NewPublicationDate WHERE id=@BookId; UPDATE books SET genre_id=@NewGenreId WHERE id=@BookId;", conn);

      SqlParameter bookIdParameter = new SqlParameter();
      bookIdParameter.ParameterName = "@BookId";
      bookIdParameter.Value = this.GetId();
      cmd.Parameters.Add(bookIdParameter);

      SqlParameter titleParameter = new SqlParameter();
      titleParameter.ParameterName = "@NewTitle";
      titleParameter.Value = newTitle;
      cmd.Parameters.Add(titleParameter);

      SqlParameter yearParameter = new SqlParameter();
      yearParameter.ParameterName = "@NewPublicationDate";
      yearParameter.Value = newPublicationDate;
      cmd.Parameters.Add(yearParameter);

      SqlParameter genreParameter = new SqlParameter();
      genreParameter.ParameterName = "@NewGenreId";
      genreParameter.Value = newGenreId;
      cmd.Parameters.Add(genreParameter);

      cmd.ExecuteNonQuery();

      if(conn!=null) conn.Close();
    }
  }
}
