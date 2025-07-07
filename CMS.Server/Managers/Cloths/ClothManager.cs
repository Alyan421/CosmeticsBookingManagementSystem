using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CMS.Server.Controllers.Cloths.DTO;
using CMS.Server.Models;
using CMS.Server.Repository;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CMS.Server.Managers.Cloths
{
    public class ClothManager : IClothManager
    {
        private readonly IGenericRepository<Cloth> _clothRepository;
        private readonly IGenericRepository<ClothColor> _clothColorRepository;
        private readonly IMapper _mapper;

        public ClothManager(
            IGenericRepository<Cloth> clothRepository,
            IGenericRepository<ClothColor> clothColorRepository,
            IMapper mapper)
        {
            _clothRepository = clothRepository;
            _clothColorRepository = clothColorRepository;
            _mapper = mapper;
        }

        public async Task<Cloth> CreateClothAsync(Cloth cloth)
        {
            try
            {
                // Add the cloth
                await _clothRepository.AddAsync(cloth);
                await _clothRepository.SaveChangesAsync();

                // Return cloth with relationships
                return await GetClothByIdAsync(cloth.Id);
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating cloth", ex);
            }
        }

        public async Task<Cloth> UpdateClothAsync(Cloth cloth)
        {
            try
            {
                // Update the cloth
                await _clothRepository.UpdateAsync(cloth);
                await _clothRepository.SaveChangesAsync();

                // Return cloth with relationships
                return await GetClothByIdAsync(cloth.Id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating cloth with ID {cloth.Id}", ex);
            }
        }

        public async Task<bool> DeleteClothAsync(int id)
        {
            try
            {
                var cloth = await _clothRepository.GetByIdAsync(id);
                if (cloth == null)
                {
                    return false; // Cloth not found
                }

                var hasAssociatedCloths = await _clothColorRepository.ExistsAsync(cc => cc.ClothId == id);
                if (hasAssociatedCloths)
                {
                    throw new InvalidOperationException("Cannot delete cloth as it is associated with one or more cloths. Please remove these associations first.");
                }

                if (cloth == null)
                {
                    return false;
                }

                await _clothRepository.DeleteAsync(cloth);
                await _clothRepository.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting cloth with ID {id}", ex);
            }
        }

        public async Task<Cloth> GetClothByIdAsync(int id)
        {
            try
            {
                return await _clothRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving cloth with ID {id}", ex);
            }
        }

        public async Task<IEnumerable<Cloth>> GetAllClothsAsync()
        {
            try
            {
                return await _clothRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving all cloths", ex);
            }
        }
    }
}