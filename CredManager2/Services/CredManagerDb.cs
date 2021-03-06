﻿using CredManager2.Models;
using Postulate.SqlCe.IntKey;
using System;
using System.Data.SqlServerCe;
using System.IO;

namespace CredManager2.Services
{
    public class CredManagerDb
    {
        private readonly string _connectionString;

        private SqlCeEngine _engine;

        public CredManagerDb(string fileName, string password)
        {
            Filename = fileName;
            _connectionString = $"Data Source='{fileName}';LCID=1033;Password={password};Encryption Mode=Platform Default";
        }

        public SqlCeConnection GetConnection()
        {
            _engine = new SqlCeEngine(_connectionString);

            if (!File.Exists(Filename))
            {
                string folder = Path.GetDirectoryName(Filename);
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                _engine.CreateDatabase();

                using (var cn = new SqlCeConnection(_connectionString))
                {
                    // Postulate CreateTable not working right because I can't get PK on the identiy
                    // using the original CredManager code here

                    cn.Open();
                    using (var createTable = new SqlCeCommand(
                        @"CREATE TABLE [Entry] (
                            [Name] nvarchar(50) NOT NULL,
                            [Url] nvarchar(100) NOT NULL,
                            [UserName] nvarchar(50) NOT NULL,
                            [Password] nvarchar(50) NOT NULL,
                            [IsActive] bit NOT NULL,
                            [DateCreated] datetime NOT NULL,
                            [DateModified] datetime NULL,
                            [Id] int identity(1,1) PRIMARY KEY
                        )", cn))
                    {
                        createTable.ExecuteNonQuery();
                    }
                }
            }

            return new SqlCeConnection(_connectionString);
        }

        public bool TryOpenConnection(out string errorMessage)
        {
            try
            {                
                using (var cn = GetConnection())
                {
                    cn.Open();
                    errorMessage = null;
                    return true;
                }
            }
            catch (Exception exc)
            {
                errorMessage = exc.Message;
                return false;
            }
        }


        public string Filename { get; }
    }
}
