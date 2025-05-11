using SQLite;

namespace MinhasAnotacoes.Models;

// Define a classe Nota que representa uma anotação
public class Nota
{
    // Define a primary key da classe Nota
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    // Define as propriedades da classe Nota
    public string Titulo { get; set; }
    public string Texto { get; set; }
    public DateTime DataCriacao { get; set; }
}