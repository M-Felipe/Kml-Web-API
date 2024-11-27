using KmlFilterAPI.models;
using KmlFilterAPI.services;
using SharpKml.Dom;
using SharpKml.Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class KmlService : IKmlService
{
    private readonly KmlFile _kmlFile;
    private readonly List<PlacemarkModel> _placemarks;

    /// <summary>
    /// Inicializa uma nova instância da classe KmlService.
    /// </summary>
    /// <param name="kmlFilePath"></param>
    /// <exception cref="FileNotFoundException"></exception>
    public KmlService(string kmlFilePath)
    {
        if (!File.Exists(kmlFilePath))
            throw new FileNotFoundException("Arquivo KML não encontrado.");

        // Carregar o arquivo KML
        using (var stream = File.OpenRead(kmlFilePath))
        {
            _kmlFile = KmlFile.Load(stream);
        }

        // Processar Placemarks
        _placemarks = ExtractPlacemarks();
    }

    /// <summary>
    /// Extrai os Placemarks do arquivo KML.
    /// </summary>
    /// <returns></returns>
    private List<PlacemarkModel> ExtractPlacemarks()
    {
        var placemarks = new List<PlacemarkModel>();

        var rootContainer = (_kmlFile.Root as Kml)?.Feature as Container;

        if (rootContainer != null)
        {
            foreach (var placemark in rootContainer.Flatten().OfType<Placemark>())
            {
                var extendedData = placemark.ExtendedData;
                var dataDictionary = extendedData?.Data.ToDictionary(d => d.Name, d => d.Value);

                placemarks.Add(new PlacemarkModel
                {
                    Cliente = dataDictionary?.GetValueOrDefault("CLIENTE"),
                    Situacao = dataDictionary?.GetValueOrDefault("SITUAÇÃO"),
                    Bairro = dataDictionary?.GetValueOrDefault("BAIRRO"),
                    Referencia = dataDictionary?.GetValueOrDefault("REFERENCIA"),
                    RuaCruzamento = dataDictionary?.GetValueOrDefault("RUA/CRUZAMENTO")
                });
            }
        }

        return placemarks;
    }

    /// <summary>
    /// Filtra os itens do Placemarks de acordo com os filtros informados.
    /// </summary>
    /// <param name="filters"></param>
    /// <returns></returns>
    public IEnumerable<PlacemarkModel> FilterPlacemarks(FilterModel filters)
    {
        ValidateFilters(filters);
        return _placemarks.Where(p =>
            (string.IsNullOrEmpty(filters.Cliente) || p.Cliente == filters.Cliente) &&
            (string.IsNullOrEmpty(filters.Situacao) || p.Situacao == filters.Situacao) &&
            (string.IsNullOrEmpty(filters.Bairro) || p.Bairro == filters.Bairro) &&
            (string.IsNullOrEmpty(filters.Referencia) || p.Referencia.Contains(filters.Referencia, StringComparison.OrdinalIgnoreCase)) &&
            (string.IsNullOrEmpty(filters.RuaCruzamento) || p.RuaCruzamento.Contains(filters.RuaCruzamento, StringComparison.OrdinalIgnoreCase))
        );
    }

    public IEnumerable<string> GetUniqueValues(string fieldName)
    {
        return fieldName.ToUpper() switch
        {
            "CLIENTE" => _placemarks.Select(p => p.Cliente).Distinct(),
            "SITUAÇÃO" => _placemarks.Select(p => p.Situacao).Distinct(),
            "BAIRRO" => _placemarks.Select(p => p.Bairro).Distinct(),
            _ => throw new ArgumentException("Campo inválido.")
        };
    }

    /// <summary>
    /// Exporta os Placemarks filtrados para um arquivo KML que é salvo na pasta temporaria do servidor.
    /// </summary>
    /// <param name="placemarks"></param>
    /// <returns></returns>
    public string ExportFilteredKml(IEnumerable<PlacemarkModel> placemarks)
    {
        var document = new Document();

        foreach (var placemark in placemarks)
        {
            var kmlPlacemark = new Placemark
            {
                Name = placemark.Cliente,
                ExtendedData = new ExtendedData()
            };

            kmlPlacemark.ExtendedData.AddData(new Data { Name = "CLIENTE", Value = placemark.Cliente });
            kmlPlacemark.ExtendedData.AddData(new Data { Name = "SITUAÇÃO", Value = placemark.Situacao });
            kmlPlacemark.ExtendedData.AddData(new Data { Name = "BAIRRO", Value = placemark.Bairro });
            kmlPlacemark.ExtendedData.AddData(new Data { Name = "REFERENCIA", Value = placemark.Referencia });
            kmlPlacemark.ExtendedData.AddData(new Data { Name = "RUA/CRUZAMENTO", Value = placemark.RuaCruzamento });

            document.AddFeature(kmlPlacemark);
        }

        var kml = new Kml { Feature = document };
        var outputFilePath = Path.Combine(Path.GetTempPath(), "FilteredPlacemarks.kml");

        using (var stream = File.Create(outputFilePath))
        {
            KmlFile.Create(kml, false).Save(stream);
        }

        return outputFilePath;
    }

    /// <summary>
    /// Valida os filtros de pesquisa.
    /// </summary>
    /// <param name="filters"></param>
    /// <exception cref="ArgumentException"></exception>
    private void ValidateFilters(FilterModel filters)
    {
        if (!string.IsNullOrEmpty(filters.Referencia) && filters.Referencia.Length < 3)
            throw new ArgumentException("O filtro 'REFERENCIA' deve ter pelo menos 3 caracteres.");

        if (!string.IsNullOrEmpty(filters.RuaCruzamento) && filters.RuaCruzamento.Length < 3)
            throw new ArgumentException("O filtro 'RUA/CRUZAMENTO' deve ter pelo menos 3 caracteres.");
    }
}
