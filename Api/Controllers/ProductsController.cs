using Api.Dtos;
using Core.Entities;
using Infra.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

/// <summary>
/// manages product operations for the current tenant
/// </summary>
[ApiController]
[Route("Api/[controller]")]
[Produces("application/json")]
public class ProductsController(ApplicationDbContext context) : ControllerBase
{
    /// <summary>
    /// retrieves all products for the authenticated tenant.
    /// </summary>
    /// <remarks>
    /// this endpoint automatically filters data based on the x-tenant-id header.
    /// </remarks>
    /// <returns>a list of products belonging to the tenant.</returns>
    /// <response code="200">returns the list of products successfully.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Product>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var products = await context.Products.ToListAsync();
        return Ok(products);
    }

    /// <summary>
    /// creates a new product for the current tenant.
    /// </summary>
    /// <remarks>
    /// requires a valid tenant header. the product name must be unique within the tenant scope.
    /// </remarks>
    /// <param name="request">the product data transfer object containing name, description and price.</param>
    /// <returns>the newly created product with its unique id.</returns>
    /// <response code="201">product created successfully.</response>
    /// <response code="400">invalid input data or tenant could not be identified.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Product), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
    {
        var product = new Product(request.Name, request.Description, request.Price);
        
        context.Products.Add(product);
        await context.SaveChangesAsync();
        
        return CreatedAtAction(nameof(GetAll), new { id = product.Id }, product);
    }
}