using Microsoft.EntityFrameworkCore;
using RecruitmentTask.Domain.Entities;
using RecruitmentTest.Application.Services;

namespace RecruitmentTest.Infrastructure;

public class DataSeeder
{
    private readonly FintachartsApiService _fintachartsApiService;
    private readonly AppDbContext _appDbContext;

    public DataSeeder(FintachartsApiService fintachartsApiService, AppDbContext appDbContext)
    {
        _fintachartsApiService = fintachartsApiService;
        _appDbContext = appDbContext;
    }

    public async Task SeedAll()
    {
        await SeedExchanges();
        await SeedKinds();
        await SeedProviders();
        await SeedInstruments();
    }

    public async Task SeedExchanges()
    {
        var data = await _fintachartsApiService.GetExchangesAsync();
        var exchanges = data.Data.Values
            .SelectMany(x => x)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct();
        
        foreach (var exchange in exchanges)
        {
            if(await _appDbContext.Exchanges.AnyAsync(e => e.Name == exchange)) continue;
            await _appDbContext.Exchanges.AddAsync(new()
            {
                Name = exchange
            });
        }
        
        await _appDbContext.SaveChangesAsync();
    }
    
    public async Task SeedKinds()
    {
        var data = await _fintachartsApiService.GetKindsAsync();
        var kinds = data.Data!;
        
        foreach (var kind in kinds)
        {
            if(_appDbContext.Kinds.Any(k => k.Name == kind)) continue;
            await _appDbContext.Kinds.AddAsync(new()
            {
                Name = kind
            });
        }
        
        await _appDbContext.SaveChangesAsync();
    }
    
    public async Task SeedProviders()
    {
        var data = await _fintachartsApiService.GetProvidersAsync();
        var providers = data.Data!;
        
        foreach (var provider in providers)
        {
            if(_appDbContext.Providers.Any(p => p.Name == provider)) continue;
            await _appDbContext.Providers.AddAsync(new()
            {
                Name = provider
            });
        }
        
        await _appDbContext.SaveChangesAsync();
    }
    
    public async Task SeedInstruments()
    {
        int page = 1;
        int pages = 2;

        while (page <= pages)
        {
            var response = await _fintachartsApiService.GetInstrumentsAsync(null, null, null, page);
            pages = response.Paging.Pages;
            
            var data = response.Data!;
            foreach (var instrument in data)
            {
                if(_appDbContext.Instruments.Any(i => i.Id == instrument.Id)) continue;

                var entity = new Instrument
                {
                    Id = instrument.Id,
                    Currency = instrument.Currency!,
                    BaseCurrency = instrument.BaseCurrency,
                    TickSize = instrument.TickSize,
                    Symbol = instrument.Symbol!,
                    Description = instrument.Description!
                };

                var kind = _appDbContext.Kinds.FirstOrDefault(k => k.Name == instrument.Kind)!;
                entity.Kind = kind;
                entity.KindId = kind.Id;

                foreach (var mapping in instrument.Mappings)
                {
                    var exchange = _appDbContext.Exchanges.FirstOrDefault(e => e.Name == mapping.Value.Exchange);
                    var provider = _appDbContext.Providers.FirstOrDefault(e => e.Name == mapping.Key)!;

                    var mappingEntity = new Mapping
                    {
                        Exchange = exchange,
                        Provider = provider,
                        DefaultOrderSize = mapping.Value.DefaultOrderSize,
                        Symbol = mapping.Value.Symbol!,
                    };
                    
                    await _appDbContext.Mappings.AddAsync(mappingEntity);
                    
                    entity.Mappings.Add(mappingEntity);
                }
                
                await _appDbContext.Instruments.AddAsync(entity);
            }
            ++page;
        }
        
        await _appDbContext.SaveChangesAsync();
    }
}
