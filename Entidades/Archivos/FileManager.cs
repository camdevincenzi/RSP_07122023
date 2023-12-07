using Entidades.Exceptions;
using Entidades.Interfaces;
using Entidades.Modelos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;

namespace Entidades.Files
{
    
    public static class FileManager
    {
        private static string path;

        static FileManager()
        {
            path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            path = Path.Combine(path, "RSP_07122023");
        }

        private static void ValidaExistenciaDeDirectorio()
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
            catch (Exception ex)
            {
                FileManager.Guardar(ex.Message, "logs.txt", true);
                throw new FileManagerException("No se encontró el directorio", ex);
            }
        }

        public static void Guardar(string data, string nombreArchivo, bool append)
        {
            try
            {
                FileManager.ValidaExistenciaDeDirectorio();

                string filePath = Path.Combine(path, nombreArchivo);

                using (StreamWriter streamWriter = new StreamWriter(filePath, append))
                {
                    streamWriter.WriteLine(data);
                }
            }
            catch (Exception ex)
            {
                FileManager.Guardar(ex.Message, "logs.txt", true);
                throw new FileManagerException("No se pudo guardar el archivo", ex);
            }
        }

        public static bool Serializar<T>(T elemento, string nombreArchivo) 
        {
            try
            {
                string filePath = Path.Combine(path, nombreArchivo);

                string json = JsonSerializer.Serialize(elemento);
                File.WriteAllText(filePath, json);

                return true;
            }
            catch (FileManagerException ex)
            {
                FileManager.Guardar(ex.Message, "logs.txt", true);
                throw new FileManagerException("Error al serializar el objeto", ex);
            }
        }
    }
}
