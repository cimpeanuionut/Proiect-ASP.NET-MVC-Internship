﻿using iWasHere.Domain.DTOs;
using iWasHere.Domain.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iWasHere.Domain.Service
{
    public class DictionaryService
    {

        private readonly BlackWidowContext _bwContext;
        private readonly DatabaseContext _databaseContext;

        public DictionaryService(BlackWidowContext databaseContext)
        {
            _bwContext = databaseContext;
        }

        public List<DictionaryLandmarkTypeModel> GetDictionaryLandmarkTypeModels()
        {

            List<DictionaryLandmarkTypeModel> dictionaryTicketModels = _databaseContext.DictionaryLandmarkType.Select(a => new DictionaryLandmarkTypeModel()
            {
                Id = a.DictionaryItemId,
                Name = a.DictionaryItemName
            }).ToList();

            return dictionaryTicketModels;
        }

        public List<DictionaryTicketModel> GetDictionaryTicketModels()
        {
           
            List<DictionaryTicketModel> dictionaryTicketModels = _bwContext.DictionaryTicket.Select(a => new DictionaryTicketModel()
            {
                DictionaryTicketId = a.DictionaryTicketId,
                TicketCategory = a.TicketCategory
            }).ToList();

            return dictionaryTicketModels;
        }  
  
        public List<DictionaryTicketModel> GetDictionaryTicketPagination(int pageSize, int page, out int noRows, string ticketType)
        {
            noRows = _bwContext.DictionaryTicket.Count();
            int skip = (page - 1) * pageSize;
            List<DictionaryTicketModel> dictionaryTicketModels = _bwContext.DictionaryTicket.Where(a=> !String.IsNullOrWhiteSpace(ticketType) ? a.TicketCategory == ticketType : a.TicketCategory ==a.TicketCategory)
                .Select(a => new DictionaryTicketModel()
            {
                DictionaryTicketId = a.DictionaryTicketId,
                TicketCategory = a.TicketCategory
            }).Skip(skip).Take(pageSize).ToList();

            return dictionaryTicketModels;
        }

        public List<DictionaryAttractionCategoryModel> GetDictionaryAttractionCategoryModels(int skip, int take, out int total, string input)
        {
            List<DictionaryAttractionCategoryModel> dictionaryAttractionCategoryModels = new List<DictionaryAttractionCategoryModel>();
            int skip_amount = (skip - 1) * take;

            IQueryable<DictionaryAttractionCategory> query = _bwContext.DictionaryAttractionCategory;
            if (!String.IsNullOrWhiteSpace(input))
            {
                query = query.Where(a => a.AttractionCategoryName.Contains(input));
            }
            total = query.Count();
            dictionaryAttractionCategoryModels = query.Select(a => new DictionaryAttractionCategoryModel()
            {
                AttractionCategoryId = a.AttractionCategoryId,
                AttractionCategoryName = a.AttractionCategoryName
            }).Skip(skip_amount).Take(take).ToList();

            return dictionaryAttractionCategoryModels;
        }

        public List<DictionaryCountryModel> GetDictionaryCountryModels(int pageSize, int page, out int total, string textBoxValue)
        {
            int skip = (page - 1) * pageSize;
            List<DictionaryCountryModel> dictionaryCountryModels = new List<DictionaryCountryModel>();
            IQueryable<DictionaryCountry> countryQuery = _bwContext.DictionaryCountry;

            if (!String.IsNullOrEmpty(textBoxValue))
            {
                countryQuery = countryQuery.Where(a => a.CountryName.Contains(textBoxValue));
            }
            dictionaryCountryModels= countryQuery.Select(a => new DictionaryCountryModel()
                {
                    Id = a.CountryId,
                    Name = a.CountryName
        }).Skip(skip).Take(pageSize).ToList();

            total=countryQuery.Count();

            return dictionaryCountryModels;

        }
        //ale lu paulica de aici

        public List<County_DTO> GetDictionaryCounty(int PageSize, int Page, out int totalRows, string f)
        {
            //f  casuta de text filtru de judet
            IQueryable<DictionaryCounty> query = _bwContext.DictionaryCounty;
            
            int skip = (Page - 1) * PageSize;
            if (!String.IsNullOrWhiteSpace(f))
            {
                query = query.Where(a => a.CountyName.ToLower().Contains(f));
            }

            totalRows = query.Count();
           
            List<County_DTO> dictionaryCounty = query
           .Select(a => new County_DTO()
           {
                    CountyId = a.CountyId,
                    CountyName = a.CountyName,
                    CountryId = a.CountryId,
                    CountryName = a.Country.CountryName
           })
                .Skip(skip).Take(PageSize).ToList();

         
             return dictionaryCounty;

        }
        public County_DTO GetCounty_ADD_UPDATE(int id)
        {

            County_DTO dictionaryCity = _bwContext.DictionaryCounty
                .Where(a => a.CountyId == id)
                .Select(a => new County_DTO()
                {
                    CountyId = a.CountyId,
                    CountyName = a.CountyName,
                    CountryId = a.CountryId,
                    CountryName = a.Country.CountryName


                }).First();

            return dictionaryCity;
        }
        public string Insert(County_DTO model)
        {
            if (String.IsNullOrWhiteSpace(model.CountyName))
            {
                return "Numele judetului este obligatoriu";
            }else if(model.CountryId == 0)
            {
                return "Nu ai selectat o tara";
            }
            try
            {
                _bwContext.DictionaryCounty.Add(new DictionaryCounty
                {

                    CountyName = model.CountyName,
                    CountryId = model.CountryId

                });
                _bwContext.SaveChanges();
                return null;
            }catch (Exception e)
            {
                return e.Message;
            }
        }
        public string Delete_County(int id)
        {
            
            string err = "";

            try
            {
                _bwContext.Remove(_bwContext.DictionaryCounty.Single(a => a.CountyId == id));
                _bwContext.SaveChanges();
                return null;
            }
            catch (Exception ex)
            {
                return "Judetul nu poate fi sters!";
            }
        }
        public string Update(County_DTO model)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(model.CountyName))
                {
                    return "Numele judetului este obligatoriu";
                }else if (model.CountryId == 0)
                {
                    return "Nu ai selectat un judet";
                }

                else
                {
                    DictionaryCounty county = _bwContext.DictionaryCounty.Find(model.CountyId);
                    county.CountyName = model.CountyName;
                    county.CountryId = model.CountryId;
                    _bwContext.Update(county);
                    _bwContext.SaveChanges();
                    return null;
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        //pana aici
        public List<DictionaryOpenSeasonModel> GetDictionaryOpenSeasonModels(int PageSize, int Page, out int totalRows, string openSeasonType)
        {
            IQueryable<DictionaryOpenSeason> query = _bwContext.DictionaryOpenSeason;
            if (!String.IsNullOrWhiteSpace(openSeasonType))
            {
                query = query.Where(a => a.OpenSeasonType.ToLower().Contains(openSeasonType));
            }
            totalRows = _bwContext.DictionaryOpenSeason.Count();
            int skip = (Page - 1) * PageSize;
            List<DictionaryOpenSeasonModel> dictionaryOpenSeasonModels = _bwContext.DictionaryOpenSeason
               .Where(a => !String.IsNullOrWhiteSpace(openSeasonType) ? a.OpenSeasonType.Contains(openSeasonType) : a.OpenSeasonType == a.OpenSeasonType)
                .Select(a => new DictionaryOpenSeasonModel()
            {
                Id = a.OpenSeasonId,
                Type = a.OpenSeasonType
            }).Skip(skip).Take(PageSize).ToList();

            return dictionaryOpenSeasonModels;
        }        

        public void DeleteOpenSeason(int id)
        {
            _bwContext.Remove(_bwContext.DictionaryOpenSeason.Single(a => a.OpenSeasonId == id));
            _bwContext.SaveChanges();
        }

        public string InsertOpenSeason(DictionaryOpenSeasonModel model)
        {
            try
            {
                _bwContext.DictionaryOpenSeason.Add(new DictionaryOpenSeason
                {
                    OpenSeasonId = model.Id,
                    OpenSeasonType = model.Type
                });
                _bwContext.SaveChanges();
                return null;
            }
            catch(Exception e)
            {
                return "Campuri necompletate.";
            } 
        }

        public DictionaryOpenSeasonModel GetOpenSeason(int id)
        {
            DictionaryOpenSeasonModel model = _bwContext.DictionaryOpenSeason.Where(a => a.OpenSeasonId == id)
                .Select(a => new DictionaryOpenSeasonModel()
                {
                    Id = a.OpenSeasonId,
                    Type = a.OpenSeasonType
                }).First();
            return model;
        }
        public string UpdateOpenSeason(DictionaryOpenSeasonModel model)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(model.Type))
                {
                    return "Numele este obligatoriu";
                }
                else
                {
                    DictionaryOpenSeason openSeason = _bwContext.DictionaryOpenSeason.Find(model.Id);
                    openSeason.OpenSeasonId = model.Id;
                    openSeason.OpenSeasonType = model.Type;

                    _bwContext.DictionaryOpenSeason.Update(openSeason);
                    _bwContext.SaveChanges();
                    return null;
                }
                
            }
            catch (Exception e)
            {
                return "Completati tipul sezonier!";
            }

        }

        public string AddAttractionCategory(string attractionCategoryName)
        {
            if (String.IsNullOrWhiteSpace(attractionCategoryName))
            {
                return "Te rog sa introduci un nume de atractie.";
            }
            try
            {
                _bwContext.DictionaryAttractionCategory.Add(new DictionaryAttractionCategory
                {
                    AttractionCategoryName = attractionCategoryName
                });

                _bwContext.SaveChanges();
                return null;
            }catch(Exception e)
            {
                return e.Message;
            }
        }

        public List<DictionaryCountryModel> GetDictionaryCountryModels()
        {

            List<DictionaryCountryModel> dictionaryCountryModels = _bwContext.DictionaryCountry.Select(a => new DictionaryCountryModel()
            {
                Id = a.CountryId,
                Name = a.CountryName
            }).ToList();

            return dictionaryCountryModels;
        }

        public void DeleteTicketType(int id)
        {
            try
            {
                _bwContext.Remove(_bwContext.DictionaryTicket.Single(a => a.DictionaryTicketId == id));
                _bwContext.SaveChanges();
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DictionaryTicketModel GetTicketFromDB(int id)
        {

            DictionaryTicketModel model = _bwContext.DictionaryTicket
                .Where(a => a.DictionaryTicketId == id)
                .Select(a => new DictionaryTicketModel()
                {
                    DictionaryTicketId = a.DictionaryTicketId,
                    TicketCategory = a.TicketCategory
                  

                }).First();

            return model;
        }

        public string UpdateTicket(DictionaryTicketModel model)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(model.TicketCategory))
                {
                    return "Tipul este obligatoriu";
                }
                else
                {
                    DictionaryTicket ticket = _bwContext.DictionaryTicket.Find(model.DictionaryTicketId);
                    ticket.DictionaryTicketId = model.DictionaryTicketId;
                    ticket.TicketCategory = model.TicketCategory;

                    _bwContext.DictionaryTicket.Update(ticket);
                    _bwContext.SaveChanges();
                    return null;
                }
            }catch (Exception e){
                return " Completati tipul biletului!";
            }
        }

        public string InsertTicket(DictionaryTicketModel model)
        {

            if (String.IsNullOrWhiteSpace(model.TicketCategory))
            {
                return "Tipul este obligatoriu";
            }
            else
            {
                _bwContext.DictionaryTicket.Add(new DictionaryTicket
                {
                    TicketCategory = model.TicketCategory,

                });
                _bwContext.SaveChanges();
                return null;
            }
        }


        public string DeleteCountry(int id)
        {
            try
            {
                _bwContext.Remove(_bwContext.DictionaryCountry.Single(a => a.CountryId == id));
                _bwContext.SaveChanges();
                return null;
            }
            catch (Exception e)
            {
                return "Aceasta tara nu poate fi stearsa";
            }
        }

        public string AddCountry(DictionaryCountryModel dictionaryCountryModel)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(dictionaryCountryModel.Name))
                {
                    return "Trebuie sa introduceti obligatoriu o tara!";
                }
                else
                {
                    _bwContext.DictionaryCountry.Add(new DictionaryCountry
                    {
                        CountryName = dictionaryCountryModel.Name,
                        CountryId = dictionaryCountryModel.Id
                    });
                    _bwContext.SaveChanges();
                    return null;
                }
            }
            catch(Exception e)
            {
                return e.Message;
            }
        }

        public DictionaryCountryModel UpdateCountry(int id)
        {

            DictionaryCountryModel dictionaryCountry = _bwContext.DictionaryCountry
                .Where(a => a.CountryId == id)
                .Select(a => new DictionaryCountryModel()
                {
                    Id = a.CountryId,
                    Name = a.CountryName,
                }).First();

            return dictionaryCountry;
        }

        public string Update(DictionaryCountryModel dictionaryCountryModel)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(dictionaryCountryModel.Name))
                {
                    return "Numele tarii este obligatoriu";
                }
                else
                {
                    DictionaryCountry dictionaryCountry = _bwContext.DictionaryCountry.Find(dictionaryCountryModel.Id);
                    dictionaryCountry.CountryName = dictionaryCountryModel.Name;
                    _bwContext.DictionaryCountry.Update(dictionaryCountry);
                    _bwContext.SaveChanges();
                    return null;
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public DictionaryAttractionCategoryModel GetAttractionCategory(int id)
        {

            DictionaryAttractionCategoryModel attractionCategory = _bwContext.DictionaryAttractionCategory
                .Where(model => model.AttractionCategoryId == id)
                .Select(model => new DictionaryAttractionCategoryModel()
                {
                    AttractionCategoryId = model.AttractionCategoryId,
                    AttractionCategoryName = model.AttractionCategoryName

                }).First();

            return attractionCategory;
        }

        public string UpdateAttractionCategory(DictionaryAttractionCategoryModel model)
        {
            if (String.IsNullOrWhiteSpace(model.AttractionCategoryName))
            {
                return "Nu ai introdus o atractie";
            }
            try
            {
                DictionaryAttractionCategory attractionCategory = _bwContext.DictionaryAttractionCategory.Find(model.AttractionCategoryId);
                attractionCategory.AttractionCategoryName = model.AttractionCategoryName;
                _bwContext.DictionaryAttractionCategory.Update(attractionCategory);
                _bwContext.SaveChanges();
                return null;
            }catch (Exception e)
            {
                return e.Message;
            }
        }

        public string DeleteAttractionCategory(int id)
        {
            try
            {
                _bwContext.Remove(_bwContext.DictionaryAttractionCategory.Single(a => a.AttractionCategoryId == id));
                _bwContext.SaveChanges();
                return "";

            }
            catch (Exception ex)
            {
                return "Nu poate fi sters!";
            }
        }

    }
}
