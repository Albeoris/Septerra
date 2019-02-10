using System;
using System.IO;
using System.Text;
using Septerra.Core.Hooks;

namespace Septerra.Core.DB
{
    public sealed class DbFsIndices : IService
    {
        public readonly String RootDirectory;
        private readonly DbFsIndicesMode _mode;
        private readonly BilateralDictionary<DbRecordId, String> _dic;
        private readonly StringBuilder _errorMessage = new StringBuilder(512);
        
        public DbFsIndices()
        {
            RootDirectory = Services<InteractionService>.Instance.DataDirectoryPath;
            _mode = DbFsIndicesMode.OnAny;

            if ((_mode & DbFsIndicesMode.OnInitialize) != 0)
                _dic = IndexDirectory(RootDirectory);
            else
                _dic = new BilateralDictionary<DbRecordId, String>();
        }

        public Boolean IsKnownPath(String filePath, out DbRecordId dbRecordId)
        {
            lock (_dic)
                return _dic.TryGetKey(filePath, out dbRecordId);
        }

        public Boolean TryGetResourcePath(DbRecordId dbRecordId, out String resourcePath)
        {
            lock (_dic)
            {
                if (_dic.TryGetValue(dbRecordId, out resourcePath))
                {
                    if (File.Exists(resourcePath))
                        return true;

                    Log.Warning($"File [{resourcePath}] is no more exists. Remove DbRecordId {dbRecordId} from cache.");
                    _dic.RemoveByKey(dbRecordId);
                }
            }

            if ((_mode & DbFsIndicesMode.OnRequest) != 0)
            {
                if (TryIndexRecord(dbRecordId, out resourcePath))
                    return true;
            }

            return false;
        }

        private Boolean TryIndexRecord(DbRecordId dbRecordId, out String resourcePath)
        {
            foreach (var currentPath in Directory.EnumerateFiles(RootDirectory, $"{dbRecordId}*", SearchOption.AllDirectories))
            {
                lock (_dic)
                    StoreResourcePath(_dic, dbRecordId, currentPath);

                resourcePath = currentPath;
                return true;
            }

            resourcePath = null;
            return false;
        }

        private BilateralDictionary<DbRecordId, String> IndexDirectory(String directoryPath)
        {
            BilateralDictionary<DbRecordId, String> dic = new BilateralDictionary<DbRecordId, String>(capacity: 7000);
            foreach (String currentPath in Directory.EnumerateFiles(RootDirectory, "????????*", SearchOption.AllDirectories))
            {
                String currentName = Path.GetFileName(currentPath);
                String prefix = currentName?.Substring(startIndex: 0, length: 8); // 0BEBC200
                if (!DbRecordId.TryParse(prefix, out var dbRecordId))
                {
                    Log.Warning($"Cannot parse DbRecordId from the name of file {currentPath}. Expected hexadecimal prefix like 0BEBC200.");
                    continue;
                }

                StoreResourcePath(dic, dbRecordId, currentPath);
            }

            return dic;
        }

        private void StoreResourcePath(BilateralDictionary<DbRecordId, String> dic, DbRecordId dbRecordId, String currentPath)
        {
            if (dic.TryGetValue(dbRecordId, out var existingPath))
            {
                String existingName = Path.GetFileName(existingPath);
                String currentName = Path.GetFileName(currentPath);

                _errorMessage.Clear();
                _errorMessage.AppendLine($"There is two files with the same DbRecordId [{dbRecordId:X8}].");
                _errorMessage.AppendLine($"Existing: {existingName} Path: {existingPath}");
                _errorMessage.AppendLine($"New: {currentName} Path: {currentPath}");
                _errorMessage.Append($"The new file [{currentName}] will replace the old one [{existingName}]. But be careful, this is probably a mistake. Only one of these files can be used by the game at the same time.");

                Log.Error(_errorMessage.ToString());
                _errorMessage.Clear();
            }

            dic.Add(dbRecordId, currentPath);
        }
    }
}