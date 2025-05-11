using MinhasAnotacoes.Models;
using SQLite;

namespace MinhasAnotacoes.Services;

public class NotaService
{
    // Cria uma conexão assíncrona com o banco de dados SQLite
    private readonly SQLiteAsyncConnection _db;

    public NotaService()
    {
        // Define o caminho do banco de dados e cria a tabela Nota
        _db = new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory, "notas.db"));
        _db.CreateTableAsync<Nota>().Wait();
    }

    // *** Define os métodos para manipular as notas ***

    // Obtem todas as notas do banco de dados, ordenadas pela data de criação
    public Task<List<Nota>> GetNotasAsync() => _db.Table<Nota>().OrderByDescending(n => n.DataCriacao).ToListAsync();

    // Obtem uma nota específica pelo ID
    public Task<Nota> GetNotaAsync(int id) => _db.Table<Nota>().FirstOrDefaultAsync(n => n.Id == id);

    // Salva ou atualiza uma nota no banco de dados
    public Task<int> SalvarNotaAsync(Nota nota) => nota.Id == 0 ? _db.InsertAsync(nota) : _db.UpdateAsync(nota);

    // Deleta uma nota do banco de dados
    public Task<int> DeletarNotaAsync(Nota nota) => _db.DeleteAsync(nota);
}
