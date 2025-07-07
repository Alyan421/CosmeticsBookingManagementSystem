using AutoMapper;
using CMS.Server.Controllers.Colors.DTO;
using CMS.Server.Models;
using CMS.Server.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CMS.Server.Managers.Colors
{
    public class ColorManager : IColorManager
    {
        private readonly IGenericRepository<Color> _colorRepository;
        private readonly IGenericRepository<ClothColor> _clothColorRepository;
        private readonly IMapper _mapper;

        public ColorManager(
            IGenericRepository<Color> colorRepository,
            IGenericRepository<ClothColor> clothColorRepository,
            IMapper mapper)
        {
            _colorRepository = colorRepository;
            _clothColorRepository = clothColorRepository;
            _mapper = mapper;
        }

        public async Task<List<Color>> GetAllColorsAsync()
        {
            try
            {
                var query = _colorRepository.GetDbSet()
                    .Include(c => c.ClothColors)
                    .ThenInclude(cc => cc.Cloth);

                return (await _colorRepository.GetListAsync(c => true)).ToList();
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception("Error retrieving colors", ex);
            }
        }

        public async Task<Color> GetColorByIdAsync(int id)
        {
            try
            {
                var color = await _colorRepository.GetByIdAsync(id);

                if (color == null)
                {
                    throw new KeyNotFoundException($"Color with ID {id} not found");
                }

                return color;
            }
            catch (KeyNotFoundException)
            {
                // Rethrow key not found exceptions as-is
                throw;
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception($"Error retrieving color with ID {id}", ex);
            }
        }

        public async Task<Color> CreateColorAsync(ColorCreateDTO colorCreateDTO)
        {
            try
            {
                // Map DTO to entity
                var color = _mapper.Map<Color>(colorCreateDTO);

                await _colorRepository.AddAsync(color);
                await _colorRepository.SaveChangesAsync();

                // Return the created color
                return await GetColorByIdAsync(color.Id);
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception("Error creating color", ex);
            }
        }

        public async Task<Color> UpdateColorAsync(ColorUpdateDTO colorUpdateDTO)
        {
            try
            {
                var color = await _colorRepository.GetByIdAsync(colorUpdateDTO.Id);
                if (color == null)
                {
                    throw new KeyNotFoundException($"Color with ID {colorUpdateDTO.Id} not found");
                }

                // Update properties
                _mapper.Map(colorUpdateDTO, color);
                await _colorRepository.UpdateAsync(color);
                await _colorRepository.SaveChangesAsync();

                // Return the updated color
                return await GetColorByIdAsync(color.Id);
            }
            catch (KeyNotFoundException)
            {
                // Rethrow key not found exceptions as-is
                throw;
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception($"Error updating color with ID {colorUpdateDTO.Id}", ex);
            }
        }

        public async Task<bool> DeleteColorAsync(int id)
        {
            try
            {
                var color = await _colorRepository.GetByIdAsync(id);
                if (color == null)
                {
                    return false; // Color not found
                }

                // Check if color is associated with any cloths
                var hasAssociatedCloths = await _clothColorRepository.ExistsAsync(cc => cc.ColorId == id);
                if (hasAssociatedCloths)
                {
                    throw new InvalidOperationException("Cannot delete color as it is associated with one or more cloths. Please remove these associations first.");
                }

                await _colorRepository.DeleteAsync(color);
                await _colorRepository.SaveChangesAsync();

                return true; // Successfully deleted
            }
            catch (InvalidOperationException)
            {
                // Rethrow business rule violations as-is
                throw;
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception($"Error deleting color with ID {id}", ex);
            }
        }
    }
}