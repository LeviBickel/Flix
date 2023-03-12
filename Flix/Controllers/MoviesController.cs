using EllipticCurve.Utils;
using Flix.Data;
using Flix.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Flix.Controllers
{
    [Authorize]
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public bool watched = false;
        public MoviesController(ApplicationDbContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: Movies
        public async Task<IActionResult> Index(string id)
        {
            ViewData["search"] = id;
            string search = id;
            var listAll = await _context.Movies.ToListAsync();

            var Watchedlist = await _context.UsersWatched.ToListAsync();
            var user = await _context.Users.FirstAsync(m => m.UserName == User.Identity.Name);
            var userID = user.Id;
            var userWatched = Watchedlist.FindAll(m => m.User == userID);

            foreach (var item in userWatched)
            {
                var watchedVideo = listAll.Find(m => m.ID == item.VideoID);
                watchedVideo.watched = true;
            }
            ScanDirectory();
            if (search == null)
            {
                List<Movies> onlyMovies = listAll.Where(m => m.Type == "Movie").OrderBy(m => m.Name).ToList();
                return View(onlyMovies);
            }
            else
            {
                List<Movies> searchRet = listAll.Where(m => m.Name.ToLower().Contains(search.ToLower()) || m.videoCategory.ToLower().Contains(search.ToLower())).OrderBy(m => m.Name).ToList();
                return View(searchRet);
            }
            
        }
        
        //GET: TV Shows
        public async Task<IActionResult> TvIndex()
        {
            var listAll = await _context.Movies.ToListAsync();

            var Watchedlist = await _context.UsersWatched.ToListAsync();
            var user = await _context.Users.FirstAsync(m => m.UserName == User.Identity.Name);
            var userID = user.Id;
            var userWatched = Watchedlist.FindAll(m => m.User == userID);

            foreach (var item in userWatched)
            {
                var watchedVideo = listAll.Find(m => m.ID == item.VideoID);
                watchedVideo.watched = true;
            }


            List<Movies> onlyTV = listAll.Where(m => m.Type == "TV Show").OrderBy(m => m.Name).ToList();
            return View(onlyTV);
        }

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movies = await _context.Movies
                .FirstOrDefaultAsync(m => m.ID == id);
            if (movies == null)
            {
                return NotFound();
            }

            return View(movies);
        }

        // GET: Movies/Create
        [Authorize(Roles = "Uploaders")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Movies/Create
        [Authorize(Roles = "Administrators")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> Create([Bind("ID,Name,Type,Video,VideoCover,Description,videoCategory")] MoviesView moviesView)
        {
                if (ModelState.IsValid)
                {
                    string uniqueFileName = null;
                    string uniqueCoverName = null;
                    string uniqueFolderName = null;
                    if (!Directory.Exists(_hostingEnvironment.WebRootPath + "\\Video"))
                    {
                        Directory.CreateDirectory(_hostingEnvironment.WebRootPath + "\\Video");
                    }
                    string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath + "\\Video");
                    string extention = Path.GetExtension(moviesView.Video.FileName);
                    string NewMovieName = moviesView.Name + extention;
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + NewMovieName;
                    uniqueFolderName = Guid.NewGuid().ToString() + "_" + moviesView.Name;
                    extention = Path.GetExtension(moviesView.VideoCover.FileName);
                    string NewCoverPhoto = moviesView.Name + extention;
                    uniqueCoverName = Guid.NewGuid().ToString() + "_" + NewCoverPhoto;
                    Directory.CreateDirectory(uploadsFolder + "\\" + uniqueFolderName);
                    string videoPath = Path.Combine(uploadsFolder + "\\" + uniqueFolderName, uniqueFileName);
                    Directory.CreateDirectory(uploadsFolder + "\\" + uniqueFolderName + "\\" + "coverImage");
                    string coverPath = Path.Combine(uploadsFolder + "\\" + uniqueFolderName + "\\" + "coverImage", uniqueCoverName);
                    using (var stream = new FileStream(coverPath, FileMode.Create))
                    {
                        await moviesView.VideoCover.CopyToAsync(stream);
                    }

                    if (moviesView.Name.Contains(" "))
                    {
                        moviesView.Name = moviesView.Name.Replace(" ", "_");
                    }
                    extention = Path.GetExtension(moviesView.Video.FileName);
                    NewMovieName = moviesView.Name + extention;
                    using (var stream = new FileStream(videoPath, FileMode.Create))
                    {
                        await moviesView.Video.CopyToAsync(stream);
                    }
                    Movies newMovie = new Movies
                    {
                        Name = moviesView.Name,
                        Type = moviesView.Type,
                        VideoPath = Path.Combine("\\Video" + "\\" + uniqueFolderName, uniqueFileName),
                        CoverPath = Path.Combine("\\Video" + "\\" + uniqueFolderName + "\\" + "coverImage", uniqueCoverName),
                        UniqueName = uniqueFolderName,
                        Description = moviesView.Description,
                        videoCategory = moviesView.videoCategory
                    };
                    _context.Add(newMovie);
                    await _context.SaveChangesAsync();

                    //TVShowRelationship newRelationship = new TVShowRelationship
                    //{
                    //    VideoID = _context.Movies.FirstOrDefaultAsync(m => m.Name == moviesView.Name).Result.ID,
                    //    ShowID = _context.Movies.FirstOrDefaultAsync(m => m.Name == moviesView.Name).Result.ID
                    //};
                    //_context.Add(newRelationship);
                    //await _context.SaveChangesAsync();
            

                int newID = _context.Movies.FirstAsync(m => m.Name == moviesView.Name).Result.ID;


                return RedirectToAction("details", new { id = newID });
            }

            
            
            return View(moviesView);
        }
        

        public async Task<bool> ScanDirectory()
        {
            /*
             * get path from the database, which is set in the administration controller.
             * check if path is accessable
             * scan directories for video files
             * hook into some imdb source material to get descriptions, cover photo, and other information
             * add movie information to the database
             * save database
             * reset interval for the poller
             * return if poll was successful
            */
            try
            {
                //remove the hosting environment addon once testing is complete.
                var path = _hostingEnvironment.WebRootPath + _context.AdminSettings.FirstOrDefault().directoryPath;
                if (!Directory.Exists(path))
                {
                    return false;
                }
                //get all video files 
                //getting all sub directories
                var ext = new List<string> { "mp4" };
                var myMovies = Directory
                    .EnumerateFiles(path, "*.*", SearchOption.AllDirectories)
                    .Where(s => ext.Contains(Path.GetExtension(s).TrimStart('.').ToLowerInvariant()));
                //get movie info from imdb source based on movie title and year
                //check whether the movie already exists in the database. If it does, update the additional movie information otherwise skip it.
                foreach (var file in myMovies)
                {
                    var movieName = file.Substring(file.LastIndexOf('/') + 1);
                    if (_context.Movies.Where(m=>m.Name == movieName).Count() < 1)
                    {
                        //check if movie folder contains a image
                        var moviePath = file.Substring(0, file.LastIndexOf("/"));
                        var photoExt = new List<string> { "jpeg", "jpg", "png" };
                        var coverPhoto = Directory.EnumerateFiles(moviePath, "*.*", SearchOption.AllDirectories).Where(s => photoExt.Contains(Path.GetExtension(s).TrimStart('.').ToLowerInvariant())).FirstOrDefault();
                        if(coverPhoto == null)
                        {
                            //import some sort of photo for the coverphoto
                            coverPhoto = "PlaceholderPath";
                        }
                        ServicePointManager.Expect100Continue = true;
                        var movieJSON = new HttpClient().GetStringAsync($"https://api.themoviedb.org/3/search/movie?api_key=c0237bf724826545f7da645d023f4ef3&language=en-US&query={movieName.Substring(0, movieName.IndexOf("."))}&page=1&include_adult=true").Result;
                        var dynamicobject = JsonConvert.DeserializeObject<dynamic>(movieJSON);
                        var description = dynamicobject.results[0].overview.ToString();
                        var type = "";
                        if (Enum.IsDefined(typeof(MovieGenres), (int) dynamicobject.results[0].genre_ids[0]))
                        {
                            type = "Movie";
                        }
                        else
                        {
                            type = "TV Show";
                        }
                        var videoCategory = "";
                        if(type == "Movie")
                        {
                            videoCategory = Enum.GetName(typeof(MovieGenres), (int)dynamicobject.results[0].genre_ids[0]);
                        }
                        else
                        {
                            videoCategory = Enum.GetName(typeof(ShowGenres), (int)dynamicobject.results[0].genre_ids[0]);
                        }
                        //this is a new movie
                        Movies newMovie = new Movies 
                        { 
                            Name = movieName,
                            Description = description,
                            CoverPath = coverPhoto,
                            videoCategory = videoCategory,
                            VideoPath = file,
                            Type = type,
                            UniqueName = movieName
                        };
                        _context.Movies.Add(newMovie);
                        _context.SaveChanges();
                    }
                    else
                    {
                        //this movie exists
                        //var movie = _context.Movies.Where(m => m.Name == movieName).First();
                        //update this movie with new imdb information if available. Maybe check if the user wants to replace by default?

                    }
                }
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        [Authorize(Roles = "Administrators")]
        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movies = await _context.Movies.FindAsync(id);
            if (movies == null)
            {
                return NotFound();
            }
            return View(movies);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrators")]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Type,VideoPath,UniqueName,CoverPath,Description,videoCategory")] Movies movies)
        {
            if (id != movies.ID)
            {
                return NotFound();
            }
            if(ModelState.IsValid)
            {
                try
                {
                    _context.Update(movies);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MoviesExists(movies.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                if(movies.Type == "Movie")
                {
                    return RedirectToAction(nameof(Index));
                }
                else if(movies.Type == "TV Show")
                {
                    return RedirectToAction(nameof(TvIndex));
                }
                else{
                    return RedirectToAction(nameof(Index));
                }
                
            }
            return View(movies);
        }

        // GET: Movies/Delete/5
        [Authorize(Roles = "Administrators")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movies = await _context.Movies
                .FirstOrDefaultAsync(m => m.ID == id);
            if (movies == null)
            {
                return NotFound();
            }

            return View(movies);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrators")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movies = await _context.Movies.FindAsync(id);
            if(Directory.Exists(_hostingEnvironment.WebRootPath + "\\Video\\" + movies.UniqueName))
            {
                System.IO.Directory.Delete(_hostingEnvironment.WebRootPath + "\\Video\\" + movies.UniqueName, true);
            }
            _context.Movies.Remove(movies);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Movies/Watch/5
        
        public async Task<IActionResult> Watch(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            
            var movie = await _context.Movies.FirstAsync(m => m.ID == id);
            UserWatchedAssociation association = new UserWatchedAssociation();
            var user = await _context.Users.FirstAsync(m => m.UserName == User.Identity.Name);
            var userID = user.Id;
            
            bool hasWatched = false;

            var watched = _context.UsersWatched.ToList().Where(m => m.User == userID);

            foreach (var item in watched)
            {
                if(item.VideoID == movie.ID)
                {
                    hasWatched = true;
                }
                else
                {
                    association.User = userID;
                    association.VideoID = movie.ID;
                }
            }
            
                

            if (movie == null)
            {
                return NotFound();
            }
            if (!hasWatched)
            {
                try
                {
                    _context.Add(association);
                    _context.SaveChanges();

                }
                catch
                {
                    throw;
                }
            }

            return View(movie);
        }

        public IActionResult TVDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movies = _context.Movies.ToList();//.FirstOrDefaultAsync(m => m.ID == id);
            var movie = movies.FirstOrDefault(m => m.ID == id);
            var associatedShows = _context.tvShowRelationships.ToList().Where(m => m.VideoID == movie.ID);
            List<Movies> episodes = new List<Movies>();
            foreach (var item in associatedShows)
            {
                var episode = movies.Find(m => m.ID == item.ShowID);
                episodes.Add(episode);
            }
            var model = new TVShowRelationshipView { Movies = movie, shows = associatedShows, Episodes = episodes };

            if (movies == null)
            {
                return NotFound();
            }

            return View(model);
        }

        public bool ScanDirectory(string Directorypath)
        {
            if (Path.Exists(Directorypath))
            {
                if (Directory.GetFiles(Directorypath).Length >= 1)
                {
                    var contents = Directory.GetFiles(Directorypath);
                    foreach (var item in contents)
                    {
                        //ID,Name,Type,Video,VideoCover,Description,videoCategory

                    }
                }
            }
            return true;
        }

        private bool MoviesExists(int id)
        {
            return _context.Movies.Any(e => e.ID == id);
        }
    }
}



enum MovieGenres
{
    Action = 28,
    Adventure = 12,
    Animation = 16,
    Comedy = 35,
    Crime = 80,
    Documentary = 99,
    Drama = 18,
    Family = 10751,
    Fantasy = 14,
    History = 36,
    Horror = 27,
    Music = 10402,
    Mystery = 9648,
    Romance = 10749,
    ScienceFiction = 878,
    TVMovie = 10770,
    Thriller = 53,
    War = 10752,
    Western = 37
}
enum ShowGenres
{
    ActionAdventure = 10759,
    Animation = 16,
    Comedy = 35,
    Crime = 80,
    Documentary = 99,
    Drama = 18,
    Family = 10751,
    Kids = 10762,
    Mystery = 9648,
    News = 10763,
    Reality = 10764,
    SciFiAndFantasy = 10765,
    Soap = 10766,
    Talk = 10767,
    WarAndPolitics = 10768,
    Western = 37
}


