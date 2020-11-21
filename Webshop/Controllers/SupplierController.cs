﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Webshop.Data;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Webshop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly Microsoft.AspNetCore.Identity.UserManager<User> _userManager;

        public SupplierController(ApplicationDbContext context, IMapper mapper, Microsoft.AspNetCore.Identity.UserManager<User> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }

        // GET: api/<SupplierController>
        [HttpGet]
        public async Task<IEnumerable<SupplierDto>> Get()
        {
            var res = await _context.Suppliers.ToListAsync();
            
            List<SupplierDto> supplierList = new List<SupplierDto>();

            foreach (Supplier r in res)
            {
                var mapppelt = _mapper.Map<SupplierDto>(r);
                supplierList.Add(mapppelt);
            }

            return supplierList;
        }

        // GET api/<SupplierController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SupplierDto>> Get(int id)
        {
            var res = await _context.Suppliers.Where(c => c.SupplierId == id).FirstOrDefaultAsync();
            if (res == null) return NotFound("The supplier you want to get is not exist.");
            var mapppelt = _mapper.Map<SupplierDto>(res);
            return mapppelt;
        }

        // POST api/<SupplierController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] SupplierDto newSupplier)
        {
            try
            {
                Supplier supplier = _mapper.Map<Supplier>(newSupplier);
                if (supplier.Name == null || supplier.Address == null) return NotFound("There was a problem with the creation: supplier name and supplier address are required.");
                _context.Suppliers.Add(supplier);
                await _context.SaveChangesAsync();
                return Ok();
            } catch(Exception ex)
            {
                return StatusCode(418, "There was a problem with the creation, please try again.");
            }
            
        }

        // PUT api/<SupplierController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] SupplierDto newSupplier)
        {
            try
            {
                var supplierWaitingForUpdate = await _context.Suppliers.FirstOrDefaultAsync(r => r.SupplierId == newSupplier.SupplierId);
                if (supplierWaitingForUpdate == null) return NotFound("The supplier you want to modify is not exist.");

                if (newSupplier.Name != null) supplierWaitingForUpdate.Name = newSupplier.Name;
                if (newSupplier.Multiplier != 0) supplierWaitingForUpdate.Multiplier = newSupplier.Multiplier;
                if (newSupplier.Address != null) supplierWaitingForUpdate.Address = newSupplier.Address;
                
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(418, "There was a problem with the modifictaion, please try again.");
            }
            
        }

        // DELETE api/<SupplierController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var dbSupplier = _context.Suppliers.SingleOrDefault(p => p.SupplierId == id);

            if (dbSupplier == null)
                return NotFound("The supplier you want to delete is not exist.");

            _context.Suppliers.Remove(dbSupplier);
            await _context.SaveChangesAsync();

            return Ok(); 
        }
    }
}
