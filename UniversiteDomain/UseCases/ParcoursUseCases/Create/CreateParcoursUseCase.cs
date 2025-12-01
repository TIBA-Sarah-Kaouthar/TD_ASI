using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.ParcoursExceptions;

namespace UniversiteDomain.UseCases.ParcoursUseCases.Create;

public class CreateParcoursUseCase
{
    private readonly IRepositoryFactory _factory;

    public CreateParcoursUseCase(IRepositoryFactory factory)
    {
        _factory = factory;
    }

    public async Task<Parcours> ExecuteAsync(string nomParcours, int anneeFormation)
    {
        var parcours = new Parcours { NomParcours = nomParcours, AnneeFormation = anneeFormation };
        return await ExecuteAsync(parcours);
    }

    public async Task<Parcours> ExecuteAsync(Parcours parcours)
    {
        await CheckBusinessRules(parcours);

        var repo = _factory.ParcoursRepository();

        Parcours p = await repo.CreateAsync(parcours);

        await _factory.SaveChangesAsync();

        return p;
    }

    private async Task CheckBusinessRules(Parcours parcours)
    {
        ArgumentNullException.ThrowIfNull(parcours);
        ArgumentNullException.ThrowIfNull(parcours.NomParcours);

        var repo = _factory.ParcoursRepository();
        ArgumentNullException.ThrowIfNull(repo);

        // Vérifier si un parcours existe déjà
        List<Parcours> existe = await repo.FindByConditionAsync(
            p => p.NomParcours.Equals(parcours.NomParcours) &&
                 p.AnneeFormation == parcours.AnneeFormation);

        if (existe is { Count: > 0 })
            throw new DuplicateParcoursException(
                $"{parcours.NomParcours} ({parcours.AnneeFormation}) - ce parcours existe déjà");

        // Nom du parcours
        if (parcours.NomParcours.Length < 3)
            throw new InvalidNomParcoursException(
                $"{parcours.NomParcours} incorrect - Le nom d'un parcours doit contenir plus de 3 caractères");

        // Année entre 1 et 5
        if (parcours.AnneeFormation < 1 || parcours.AnneeFormation > 5)
            throw new InvalidAnneeFormationException(
                $"{parcours.AnneeFormation} incorrecte - L'année de formation doit être comprise entre 1 et 5");
    }
}
