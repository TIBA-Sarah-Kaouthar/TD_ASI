using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.UeExceptions;

namespace UniversiteDomain.UseCases.UeUseCases;

public class CreateUeUseCase
{
    private readonly IRepositoryFactory _factory;

    public CreateUeUseCase(IRepositoryFactory factory)
    {
        _factory = factory;
    }

    public async Task<Ue> ExecuteAsync(string numeroUe, string intitule)
    {
        var ue = new Ue
        {
            NumeroUe = numeroUe,
            Intitule = intitule
        };

        return await ExecuteAsync(ue);
    }

    public async Task<Ue> ExecuteAsync(Ue ue)
    {
        await CheckBusinessRules(ue);

        var repo = _factory.UeRepository();

        Ue saved = await repo.CreateAsync(ue);

        await _factory.SaveChangesAsync();

        return saved;
    }

    private async Task CheckBusinessRules(Ue ue)
    {
        ArgumentNullException.ThrowIfNull(ue);
        ArgumentNullException.ThrowIfNull(ue.NumeroUe);
        ArgumentNullException.ThrowIfNull(ue.Intitule);

        var repo = _factory.UeRepository();

        // Vérifier si une UE avec le même NumeroUe existe déjà
        List<Ue> exist =
            await repo.FindByConditionAsync(u => u.NumeroUe.Equals(ue.NumeroUe));

        if (exist is { Count: > 0 })
            throw new DuplicateUeException(
                $"Une UE avec le numéro {ue.NumeroUe} existe déjà");

        // Vérifier que l'intitulé > 3 caractères
        if (ue.Intitule.Length < 3)
            throw new InvalidIntituleUeException(
                $"{ue.Intitule} est incorrect - un intitulé doit contenir plus de 3 caractères");
    }
}