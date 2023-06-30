using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace DataDesensitizer.FieldTypeProcessors;

public abstract class SourceFileFieldTypeProcessorBase
{
    private readonly FieldTypeProcessorSettings _settings;
    private readonly ILogger _logger;
    private System.IO.StreamReader? _reader;

    public SourceFileFieldTypeProcessorBase(FieldTypeProcessorSettings settings, ILogger logger)
    {
        _settings = settings;
        _logger = logger;
    }
    protected abstract string Filename { get; }
    protected virtual string GetFilePath()
    {
        return Path.Combine(_settings.DataFilesPath, this.Filename);
    }

    public virtual object? GetNewValue(Models.ColumnSettingModel columnSetting, SqlDataReader dataReader)
    {
        //I could support things like "randomize" but is that really necessary?

        EnsureNamesLoaded();

        return _reader!.ReadLine();
    }

    private void EnsureNamesLoaded()
    {
        if (_reader != null)
        {
            if (_reader.EndOfStream)
            {
                //TODO: warn that we are starting over
                // well, if it  doesn't have to be unique, then that's fine
                //  but if it does need to be unique, that's and issue
                _reader.Dispose();
                _reader = null;
            }
            else
            {
                return;
            }
        };

        _reader = System.IO.File.OpenText(GetFilePath());
    }

    #region Dispose
    private bool _disposedValue;

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _reader?.Dispose();
                _reader = null;
            }

            _disposedValue = true;
        }
    }
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    #endregion
}
