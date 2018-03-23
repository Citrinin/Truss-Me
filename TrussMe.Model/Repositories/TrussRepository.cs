using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AutoMapper;
using TrussMe.DataAccess.EFContext;
using TrussMe.Model.Entities;
using TrussMe.Model.Interfaces;
using TrussDA = TrussMe.DataAccess.Entities.Truss;
using BarDA = TrussMe.DataAccess.Entities.Bar;

namespace TrussMe.Model.Repositories
{
    public class TrussRepository : ITrussRepository
    {
        private readonly TrussContext _trussContext;

        public TrussRepository(TrussContext trussContext)
        {
            this._trussContext = trussContext;
        }

        public void Add(Truss item)
        {
            var addTruss = Mapper.Map<TrussDA>(item);
            var searchTruss = _trussContext.Truss.FirstOrDefault(truss =>
                truss.Span == addTruss.Span &&
                Math.Abs(truss.Slope - addTruss.Slope) < 0.001 &&
                truss.PanelAmount == addTruss.PanelAmount &&
                truss.LoadId == addTruss.LoadId &&
                truss.UnitForce == addTruss.UnitForce &&
                truss.SupportDepth == addTruss.SupportDepth);
            if (searchTruss == null)
            {
                if (addTruss.UnitForce)
                {
                    // MessageBox.Show("No such truss in DB! Unit force will change to Actual force. Enter values manually");
                    addTruss.UnitForce = false;
                }

                _trussContext.Truss.Add(addTruss);
                //context.SaveChanges();
                for (var i = 1; i <= addTruss.PanelAmount / 2; i++)
                {

                    _trussContext.Bar.Add(new BarDA
                    {
                        TrussId = addTruss.TrussId,
                        ElementId = _trussContext.TrussElement.First(el => el.ShortName == "ВП").ElementId,
                        BarNumber = i
                    });

                    _trussContext.Bar.Add(new BarDA
                    {
                        TrussId = addTruss.TrussId,
                        ElementId = _trussContext.TrussElement.First(el => el.ShortName == "НП").ElementId,
                        BarNumber = addTruss.PanelAmount / 2 + i
                    });

                    if (i == 1)
                    {
                        _trussContext.Bar.Add(new BarDA
                        {
                            TrussId = addTruss.TrussId,
                            ElementId = _trussContext.TrussElement.First(el => el.ShortName == "ОР").ElementId,
                            BarNumber = addTruss.PanelAmount + 2 * i - 1
                        });
                        _trussContext.Bar.Add(new BarDA
                        {
                            TrussId = addTruss.TrussId,
                            ElementId = _trussContext.TrussElement.First(el => el.ShortName == "ОР").ElementId,
                            BarNumber = addTruss.PanelAmount + 2 * i
                        });
                    }
                    else
                    {
                        _trussContext.Bar.Add(new BarDA
                        {
                            TrussId = addTruss.TrussId,
                            ElementId = _trussContext.TrussElement.First(el => el.ShortName == "Р").ElementId,
                            BarNumber = addTruss.PanelAmount + 2 * i - 1
                        });
                        _trussContext.Bar.Add(new BarDA
                        {
                            TrussId = addTruss.TrussId,
                            ElementId = _trussContext.TrussElement.First(el => el.ShortName == "Р").ElementId,
                            BarNumber = addTruss.PanelAmount + 2 * i
                        });
                    }
                }

                _trussContext.SaveChanges();
                item.TrussId = Mapper.Map<Truss>(addTruss).TrussId;
                item.Bar = new ObservableCollection<Bar>(addTruss.Bar.Select(Mapper.Map<Bar>));
            }
            else
            {
                //item = Mapper.Map<Entities.Truss>(searchTruss);
                item.Bar = new ObservableCollection<Bar>(searchTruss.Bar.Select(Mapper.Map<Bar>));
                item.TrussId = searchTruss.TrussId;
            }
        }

        public IEnumerable<Truss> GetAll()
        {
            return _trussContext.Truss.ToArray().Select(Mapper.Map<Truss>);
        }

        public void Remove(Truss itemToDelete)
        {
            var dbTruss = _trussContext.Truss.First(tr => tr.TrussId == itemToDelete.TrussId);
            _trussContext.Bar.RemoveRange(dbTruss.Bar);

            _trussContext.ProjectTruss.RemoveRange(dbTruss.ProjectTruss);
            _trussContext.Truss.Remove(dbTruss);
            _trussContext.SaveChanges();
        }

        public void Update(Truss item)
        {
            var dbTruss = _trussContext.Truss.First(tr => tr.TrussId == item.TrussId);
            if (dbTruss.PanelAmount != item.PanelAmount)
            {
                dbTruss.PanelAmount = item.PanelAmount;
                _trussContext.Bar.RemoveRange(dbTruss.Bar);
                for (int i = 1; i <= item.PanelAmount / 2; i++)
                {

                    _trussContext.Bar.Add(new BarDA
                    {
                        TrussId = item.TrussId,
                        ElementId = _trussContext.TrussElement.First(el => el.ShortName == "ВП").ElementId,
                        BarNumber = i
                    });

                    _trussContext.Bar.Add(new BarDA
                    {
                        TrussId = item.TrussId,
                        ElementId = _trussContext.TrussElement.First(el => el.ShortName == "НП").ElementId,
                        BarNumber = item.PanelAmount / 2 + i
                    });

                    if (i == 1)
                    {
                        _trussContext.Bar.Add(new BarDA
                        {
                            TrussId = item.TrussId,
                            ElementId = _trussContext.TrussElement.First(el => el.ShortName == "ОР").ElementId,
                            BarNumber = item.PanelAmount + 2 * i - 1
                        });
                        _trussContext.Bar.Add(new BarDA
                        {
                            TrussId = item.TrussId,
                            ElementId = _trussContext.TrussElement.First(el => el.ShortName == "ОР").ElementId,
                            BarNumber = item.PanelAmount + 2 * i
                        });
                    }
                    else
                    {
                        _trussContext.Bar.Add(new BarDA
                        {
                            TrussId = item.TrussId,
                            ElementId = _trussContext.TrussElement.First(el => el.ShortName == "Р").ElementId,
                            BarNumber = item.PanelAmount + 2 * i - 1
                        });
                        _trussContext.Bar.Add(new BarDA
                        {
                            TrussId = item.TrussId,
                            ElementId = _trussContext.TrussElement.First(el => el.ShortName == "Р").ElementId,
                            BarNumber = item.PanelAmount + 2 * i
                        });
                    }
                }
            }
            dbTruss.Span = item.Span;
            dbTruss.Slope = item.Slope;
            dbTruss.SupportDepth = item.SupportDepth;
            dbTruss.LoadId = item.LoadId;
            _trussContext.SaveChanges();
            item.Bar = new ObservableCollection<Bar>(dbTruss.Bar.Select(Mapper.Map<Bar>));
        }

        public IEnumerable<TypeOfLoad> GetTypeOfLoads()
        {
            return _trussContext.TypeOfLoad.ToArray().Select(Mapper.Map<TypeOfLoad>);

        }

        public IEnumerable<BarTemplate> GetBarTypes()
        {
            return _trussContext.TrussElement.ToArray().Select(Mapper.Map<BarTemplate>).Distinct();
        }
    }
}
