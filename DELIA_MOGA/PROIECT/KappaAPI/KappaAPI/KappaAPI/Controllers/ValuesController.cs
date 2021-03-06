﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System;
using KappaAPI.Classes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Linq;

namespace KappaAPI.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        // GET api/values
        public static string _connectionString = "Server=tcp:iogrserver.database.windows.net,1433;Initial Catalog=IoGrDatabase;Persist Security Info=False;User ID=IonutGrad;Password=GradIonut1;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";


        [HttpGet]
        public string Get() // get data from Date
        {
            SqlConnection DBConn = new SqlConnection(_connectionString);
            SqlCommand getCommand = null;
            SqlDataReader reader;
            List<Date> dataFromTable = new List<Date>();
            try
            {
                DBConn.Open();
                string getDataFromDateTable = "SELECT * FROM Date";
                getCommand = new SqlCommand(getDataFromDateTable, DBConn);
                reader = getCommand.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        dataFromTable.Add(new Date
                        {
                            Latitude = Convert.ToDouble(reader["Latitude"]),
                            Longitude = Convert.ToDouble(reader["Longitude"]),
                            Temperature = Convert.ToDouble(reader["Temperature"]),
                            Humidity = Convert.ToDouble(reader["Humidity"]),
                            Data = Convert.ToDateTime(reader["Data"]),
                            NeedIrigation = reader["NeedIrigation"].ToString()
                        });
                    }
                }
            }
            catch (Exception exp)
            {
                return exp.Message.ToString();
            }
            var serializedJson = JsonConvert.SerializeObject(dataFromTable);

            return serializedJson;
        }

        // GET api/values/5
        [HttpGet("{zona}")]
        public string Get(string zona) // get data from Zone
        {
            SqlConnection DBConn = new SqlConnection(_connectionString);
            SqlCommand getCommand = null;
            SqlDataReader reader;
            List<Zone> dataFromZoneTable = new List<Zone>();
            try
            {
                DBConn.Open();
                string getDataFromDateTable = "SELECT * FROM Zone";
                getCommand = new SqlCommand(getDataFromDateTable, DBConn);
                reader = getCommand.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        dataFromZoneTable.Add(new Zone
                        {
                            Zona = Convert.ToInt32(reader["Zona"]),
                            Latitude = Convert.ToDouble(reader["Latitude"]),
                            Longitude = Convert.ToDouble(reader["Longitude"])
                        });
                    }
                }
            }
            catch (Exception exp)
            {
                return exp.Message.ToString();
            }
            var serializedJson = JsonConvert.SerializeObject(dataFromZoneTable);

            return serializedJson;
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] dynamic fromBody)
        {
            IEnumerable<DateDePrelucrat> IdateDePrelucrat;
            IEnumerable<IntervalDeDate> IintervalDeDate;
            SqlConnection DBConn = null;
            SqlCommand insertCommand = null;
            SqlCommand deleteCommand = null;
            string value = Convert.ToString(fromBody);

            try
            {
                IintervalDeDate = JsonConvert.DeserializeObject<IEnumerable<IntervalDeDate>>(value);
                if( IintervalDeDate == null || (IintervalDeDate.First().TemperatureMax == 0 && IintervalDeDate.First().TemperatureMin == 0  &&
                                                IintervalDeDate.First().HumidityMax == 0 && IintervalDeDate.First().HumidityMin == 0))
                {
                    try
                    {
                        //value = request.Content.ReadAsStringAsync().Result;
                        IdateDePrelucrat = JsonConvert.DeserializeObject<IEnumerable<DateDePrelucrat>>(value);
                        DBConn = new SqlConnection(_connectionString);
                        DBConn.Open();
                        foreach (var item in IdateDePrelucrat)
                        {
                            try
                            {
                                string insertCmd = string.Format
                                    (
                                        "INSERT INTO DateDePrelucrat VALUES({0},{1},{2},{3})",
                                        item.Field, item.Temperature, item.Humidity, "'" + item.Date_Time + "'"
                                    );
                                insertCommand = new SqlCommand(insertCmd, DBConn);
                                insertCommand.ExecuteNonQuery();
                                deleteCommand.Dispose();
                            }
                            catch (Exception exp)
                            {
                                exp.Message.ToString();
                            } // try to insert the next data
                        }
                    }
                    catch (Exception exp)
                    {
                        exp.Message.ToString();
                    }
                }
                else
                {
                    DBConn = new SqlConnection(_connectionString);
                    DBConn.Open();
                    string deleteCmd = "DELETE FROM IntervalDeDate";
                    string insertCmd = string.Format
                            (
                                "INSERT INTO IntervalDeDate VALUES({0},{1},{2},{3})",
                                IintervalDeDate.First().TemperatureMin, IintervalDeDate.First().TemperatureMax, IintervalDeDate.First().HumidityMin, IintervalDeDate.First().HumidityMax

                            );
                    insertCommand = new SqlCommand(insertCmd, DBConn);
                    deleteCommand = new SqlCommand(deleteCmd, DBConn);
                    deleteCommand.ExecuteNonQuery();
                    insertCommand.ExecuteNonQuery();
                    insertCommand.Dispose();
                    deleteCommand.Dispose();
                }
            }
            catch (Exception exp)
            {
                exp.Message.ToString();
            }
            finally
            {
                if (DBConn != null)
                    DBConn.Dispose();
            }

        }


        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
