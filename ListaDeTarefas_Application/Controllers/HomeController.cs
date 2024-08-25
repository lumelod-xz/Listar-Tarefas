using ListaDeTarefas_Application.Data;
using ListaDeTarefas_Application.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ListaDeTarefas_Application.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;   
        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(string id)
        {
            var filtros = new Filtros(id);

            ViewBag.Filtros = filtros;
            ViewBag.Categorias = _context.Categoria.ToList();
            ViewBag.Status = _context.Status.ToList();
            ViewBag.VencimentoValores = Filtros.VencimentoValoresFiltro;

            IQueryable<Tarefa> consulta = _context.Tarefa
                .Include(c => c.Categoria)
                .Include(s => s.Status);

            if (filtros.TemCategoria)
            {
                consulta = consulta.Where(t => t.CategoriaId == filtros.CategoriaId);
            }

            if (filtros.TemStatus)
            {
                consulta = consulta.Where(t => t.StatusId == filtros.StatusId);
            }

            if (filtros.TemVencimento)
            {
                var hoje = DateTime.Today;

                if (filtros.EPassado)
                {
                    consulta = consulta.Where(t => t.DataVencimento < hoje);
                }

                if (filtros.EFuturo)
                {
                    consulta = consulta.Where(t => t.DataVencimento > hoje);
                }

                if (filtros.EHoje)
                {
                    consulta = consulta.Where(t => t.DataVencimento == hoje);
                }
            }

            var tarefas = consulta.OrderBy(t => t.DataVencimento).ToList();

            return View(tarefas);
        }

        public IActionResult Adicionar()
        {
            ViewBag.Categorias = _context.Categoria.ToList();
            ViewBag.Status = _context.Status.ToList();
            var tarefa = new Tarefa
            {
                StatusId = "aberto"
            };

            return View(tarefa);
        }

        [HttpPost]
        public IActionResult Filtrar(string[] filtro)
        {
            string id = string.Join('-', filtro);
            return RedirectToAction("Index", new { ID = id });
        }

        [HttpPost]
        public IActionResult MarcarCompleto([FromRoute] string id, Tarefa tarefaSelecionada)
        {
            tarefaSelecionada = _context.Tarefa.Find(tarefaSelecionada.Id);

            if(tarefaSelecionada != null)
            {
                tarefaSelecionada.StatusId = "completo";
                _context.SaveChanges();
            }

            return RedirectToAction("Index", new { ID = id });
        }

        [HttpPost]
        public IActionResult Adicionar(Tarefa tarefa)
        {
            if (ModelState.IsValid)
            {
                _context.Tarefa.Add(tarefa);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }
            else{
                ViewBag.Categorias = _context.Categoria.ToList();
                ViewBag.Status = _context.Status.ToList();

                return View(tarefa);
            }
        }

        [HttpPost]
        public IActionResult DeletarCompletos(string id)
        {
            var paraDeletar = _context.Tarefa.Where(s => s.StatusId == "completo").ToList();
            
            foreach (var tarefa in paraDeletar)
            {
                _context.Tarefa.Remove(tarefa);
            }
            _context.SaveChanges();

            return RedirectToAction("Index", new {ID = id});
        }
    }
}
