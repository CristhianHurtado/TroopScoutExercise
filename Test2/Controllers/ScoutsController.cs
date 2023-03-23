using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using Test2.Data;
using Test2.Models;
using Test2.ViewModels;
using static System.Formats.Asn1.AsnWriter;

namespace Test2.Controllers
{
    public class ScoutsController : Controller
    {
        private readonly ScoutTroopContext _context;

        public ScoutsController(ScoutTroopContext context)
        {
            _context = context;
        }

        // GET: Scouts
        public async Task<IActionResult> Index(string SearchString, int? TroopID, 
            string actionButton, string sortDirection = "asc", string sortField = "Scout")
        {
            //Toggle the Open/Closed state of the collapse depending on if we are filtering
            ViewData["Filtering"] = ""; //Assume not filtering
            //Then in each "test" for filtering, add ViewData["Filtering"] = " show" if true;

            PopulateDropDownLists();

            //List of sort options.
            //NOTE: make sure this array has matching values to the column headings
            string[] sortOptions = new[] { "Scout", "Age", "Fee Paid", "Troop Number", "Troop Name" };

            var scouts = _context.Scouts
                .Include(s => s.Troop)
                .Include(s => s.ScoutThumbnail)
                .AsNoTracking();

            //Add as many filters as needed
            if (TroopID.HasValue)
            {
                scouts = scouts.Where(p => p.TroopID == TroopID);
                ViewData["Filtering"] = " show";
            }
            if (!String.IsNullOrEmpty(SearchString))
            {
                scouts = scouts.Where(p => p.LastName.ToUpper().Contains(SearchString.ToUpper())
                                       || p.FirstName.ToUpper().Contains(SearchString.ToUpper()));
                ViewData["Filtering"] = " show";
            }

            //Before we sort, see if we have called for a change of filtering or sorting
            if (!String.IsNullOrEmpty(actionButton)) //Form Submitted!
            {
                if (sortOptions.Contains(actionButton))//Change of sort is requested
                {
                    if (actionButton == sortField) //Reverse order on same field
                    {
                        sortDirection = sortDirection == "asc" ? "desc" : "asc";
                    }
                    sortField = actionButton;//Sort by the button clicked
                }
            }
            //Now we know which field and direction to sort by
            if (sortField == "Fee Paid")
            {
                if (sortDirection == "asc")
                {
                    scouts = scouts
                        .OrderBy(p => p.FeePaid);
                }
                else
                {
                    scouts = scouts
                        .OrderByDescending(p => p.FeePaid);
                }
            }
            else if (sortField == "Age")
            {
                if (sortDirection == "asc")
                {
                    scouts = scouts
                        .OrderByDescending(p => p.DOB);
                }
                else
                {
                    scouts = scouts
                        .OrderBy(p => p.DOB);
                }
            }
            else if (sortField == "Troop Number")
            {
                if (sortDirection == "asc")
                {
                    scouts = scouts
                        .OrderBy(p => p.Troop.TroopNumber);
                }
                else
                {
                    scouts = scouts
                        .OrderByDescending(p => p.Troop.TroopNumber);
                }
            }
            else if (sortField == "Troop Name")
            {
                if (sortDirection == "asc")
                {
                    scouts = scouts
                        .OrderBy(p => p.Troop.TroopName);
                }
                else
                {
                    scouts = scouts
                        .OrderByDescending(p => p.Troop.TroopName);
                }
            }
            else //Sorting by Scout Name
            {
                if (sortDirection == "asc")
                {
                    scouts = scouts
                        .OrderBy(p => p.LastName)
                        .ThenBy(p => p.FirstName);
                }
                else
                {
                    scouts = scouts
                        .OrderByDescending(p => p.LastName)
                        .ThenByDescending(p => p.FirstName);
                }
            }
            //Set sort for next time
            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;

            return View(await scouts.ToListAsync());
        }

        // GET: Scouts/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Scouts == null)
            {
                return NotFound();
            }

            var scout = await _context.Scouts
                .Include(s => s.Troop)
                .Include(s => s.ScoutPhoto)
                .Include(s=>s.Achievements)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (scout == null)
            {
                return NotFound();
            }

            return View(scout);
        }

        // GET: Scouts/Create
        [Authorize(Roles = "Admin, Leader, Staff, Assistant")]
        public IActionResult Create()
        {
            PopulateDropDownLists();
            return View();
        }

        // POST: Scouts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Leader, Staff, Assistant")]
        public async Task<IActionResult> Create([Bind("ID,FirstName,LastName,DOB,FeePaid,TroopID")] Scout scout, IFormFile thePicture)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await AddPicture(scout, thePicture);
                    _context.Add(scout);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed"))
                {
                    ModelState.AddModelError("", "Unable to save changes. Remember, you cannot have duplicate Scouts (First name, Last name and DOB)");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }

            PopulateDropDownLists(scout);
            return View(scout);
        }

        // GET: Scouts/Edit/5
        [Authorize(Roles = "Admin, Leader, Staff, Assistant")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Scouts == null)
            {
                return NotFound();
            }

            var scout = await _context.Scouts
                .Include(s => s.ScoutPhoto)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (scout == null)
            {
                return NotFound();
            }

            //prevents edit on not self created
            if (User.IsInRole("Staff"))
            {
                if (scout.CreatedBy != User.Identity.Name)
                {
                    ModelState.AddModelError("", "As a Staff member, you cannot edit this scout due to you didn't create this scout");
                }
            }
            PopulateDropDownLists(scout);
            return View(scout);
        }

        // POST: Scouts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Leader, Staff, Assistant")]
        public async Task<IActionResult> Edit(int id, string chkRemoveImage, IFormFile thePicture, Byte[] RowVersion)
        {
            //Get the Scout to update
            var scoutToUpdate = await _context.Scouts
                .Include(s => s.ScoutPhoto)
                .FirstOrDefaultAsync(m => m.ID == id);

            //Check that you got it or exit with a not found error
            if (scoutToUpdate == null)
            {
                return NotFound();
            }

            //prevents edit on not self created
            if (User.IsInRole("Staff"))
            {
                if (scoutToUpdate.CreatedBy != User.Identity.Name)
                {
                    ModelState.AddModelError("", "As a Staff member, you cannot edit this scout due to you didn't create this scout");
                    return View(scoutToUpdate);
                }
            }

            _context.Entry(scoutToUpdate).Property("RowVersion").OriginalValue = RowVersion;

            if (await TryUpdateModelAsync<Scout>(scoutToUpdate, "",
                p => p.FirstName, p => p.LastName, p => p.FeePaid, 
                p => p.DOB, p => p.TroopID))
            {
                try
                {
                    //For the image
                    if (chkRemoveImage != null)
                    {
                        //If we are just deleting the two versions of the photo, we need to make sure the Change Tracker knows
                        //about them both so go get the Thumbnail since we did not include it.
                        scoutToUpdate.ScoutThumbnail = _context.scoutThumbnails.Where(p => p.ScoutID == scoutToUpdate.ID).FirstOrDefault();
                        //Then, setting them to null will cause them to be deleted from the database.
                        scoutToUpdate.ScoutPhoto = null;
                        scoutToUpdate.ScoutThumbnail = null;
                    }
                    else
                    {
                        await AddPicture(scoutToUpdate, thePicture);
                    }
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ScoutExists(scoutToUpdate.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "The record you attempted to edit "
                            + "was modified by another user. Please go back and refresh."); 
                    }
                }
                catch (DbUpdateException dex)
                {
                    if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed"))
                    {
                        ModelState.AddModelError("", "Unable to save changes. Remember, you cannot have duplicate Scouts (First name, Last name and DOB)");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                    }
                }
            }
            PopulateDropDownLists(scoutToUpdate);
            return View(scoutToUpdate);
        }

        // GET: Scouts/Delete/5
        [Authorize(Roles = "Admin, Leader")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Scouts == null)
            {
                return NotFound();
            }

            var scout = await _context.Scouts
                .Include(s => s.Troop)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (scout == null)
            {
                return NotFound();
            }

            //prevents delete on non self created
            if (User.IsInRole("Leader"))
            {
                if (scout.CreatedBy != User.Identity.Name)
                {
                    ModelState.AddModelError("", "As a Leader, you cannot edit this scout due to you didn't create this scout");
                }
            }

            return View(scout);
        }

        // POST: Scouts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Leader")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Scouts == null)
            {
                return Problem("Entity set 'ScoutTroopContext.Scouts'  is null.");
            }
            var scout = await _context.Scouts.FindAsync(id);

            //prevents delete on non self created
            if (User.IsInRole("Leader"))
            {
                if (scout.CreatedBy != User.Identity.Name)
                {
                    ModelState.AddModelError("", "As a Leader, you cannot edit this scout due to you didn't create this scout");
                    return View(scout);
                }
            }

            try
            {
                if (scout != null)
                {
                    _context.Scouts.Remove(scout);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                //Note: there is really no reason a delete should fail if you can "talk" to the database.
                ModelState.AddModelError("", "Unable to delete record. Try again, and if the problem persists see your system administrator.");
            }
            return View(scout);

        }

        [Authorize]
        public IActionResult AchievementSummary()
        {
            var sumQ = _context.Achievements.Include(a => a.Scout).ThenInclude(a=>a.Troop)
                .GroupBy(a => new { a.ScoutID, a.Scout.LastName, a.Scout.FirstName, a.Scout.Troop.TroopName })
                .Select(grp => new AchievementSummaryVM
                {
                    ID = grp.Key.ScoutID,
                    FirstName = grp.Key.FirstName,
                    LastName = grp.Key.LastName,
                    TroopName = grp.Key.TroopName,
                    NumberOfYears = grp.Count(),
                    TotalHL = grp.Sum(a => a.HL),
                    AverageLS = grp.Average(a => a.LS),
                    MaxEO = grp.Max(a => a.EO)
                }).OrderBy(s => s.LastName).ThenBy(s => s.FirstName);

            return View(sumQ.AsNoTracking().ToList());
        }


        public IActionResult DownloadAchievements()
        {
            //Get the appointments
            var appts = from a in _context.Achievements
                        .Include(a => a.Scout)
                        orderby a.ScoutID
                        select new
                        {
                            Name = a.Scout.FullName,
                            Troop = a.Scout.Troop.TroopName,
                            NumberOfYears = a.Year,
                            TotalHL = a.HL,
                            AverageLS = a.LS,
                            MaxEO = a.EO
                        };
            //How many rows?
            int numRows = appts.Count();

            if (numRows > 0) //We have data
            {
                //Create a new spreadsheet from scratch.
                using (ExcelPackage excel = new ExcelPackage())
                {

                    //Note: you can also pull a spreadsheet out of the database if you
                    //have saved it in the normal way we do, as a Byte Array in a Model
                    //such as the UploadedFile class.
                    //
                    // Suppose...
                    //
                    // var theSpreadsheet = _context.UploadedFiles.Include(f => f.FileContent).Where(f => f.ID == id).SingleOrDefault();
                    //
                    //    //Pass the Byte[] FileContent to a MemoryStream
                    //
                    // using (MemoryStream memStream = new MemoryStream(theSpreadsheet.FileContent.Content))
                    // {
                    //     ExcelPackage package = new ExcelPackage(memStream);
                    // }

                    var workSheet = excel.Workbook.Worksheets.Add("Achivements");

                    //Note: Cells[row, column]
                    workSheet.Cells[3, 1].LoadFromCollection(appts, true);

                    //Style first column for dates
                    workSheet.Column(3).Style.Numberformat.Format = "###000";

                    workSheet.Column(4).Style.Numberformat.Format = "###000";

                    //Style fee column for currency
                    workSheet.Column(5).Style.Numberformat.Format = "###,##0.00";

                    workSheet.Column(6).Style.Numberformat.Format = "###,##0.00";


                    //Note: You can define a BLOCK of cells: Cells[startRow, startColumn, endRow, endColumn]
                    //Make Date and Patient Bold
                    workSheet.Cells[4, 1, numRows + 3, 2].Style.Font.Bold = true;

                    //Note: these are fine if you are only 'doing' one thing to the range of cells.
                    //Otherwise you should USE a range object for efficiency
                    using (ExcelRange totalfees = workSheet.Cells[numRows + 4, 4])//
                    {
                        totalfees.Formula = "Sum(" + workSheet.Cells[4, 4].Address + ":" + workSheet.Cells[numRows + 3, 4].Address + ")";
                        totalfees.Style.Font.Bold = true;
                        totalfees.Style.Numberformat.Format = "###,##0.00";
                    }

                    //Set Style and backgound colour of headings
                    using (ExcelRange headings = workSheet.Cells[3, 1, 3, 6])
                    {
                        headings.Style.Font.Bold = true;
                        var fill = headings.Style.Fill;
                        fill.PatternType = ExcelFillStyle.Solid;
                        fill.BackgroundColor.SetColor(Color.LightBlue);
                    }

                    ////Boy those notes are BIG!
                    ////Lets put them in comments instead.
                    //for (int i = 4; i < numRows + 4; i++)
                    //{
                    //    using (ExcelRange Rng = workSheet.Cells[i, 7])
                    //    {
                    //        string[] commentWords = Rng.Value.ToString().Split(' ');
                    //        Rng.Value = commentWords[0] + "...";
                    //        //This LINQ adds a newline every 7 words
                    //        string comment = string.Join(Environment.NewLine, commentWords
                    //            .Select((word, index) => new { word, index })
                    //            .GroupBy(x => x.index / 7)
                    //            .Select(grp => string.Join(" ", grp.Select(x => x.word))));
                    //        ExcelComment cmd = Rng.AddComment(comment, "Apt. Notes");
                    //        cmd.AutoFit = true;
                    //    }
                    //}

                    //Autofit columns
                    workSheet.Cells.AutoFitColumns();
                    //Note: You can manually set width of columns as well
                    //workSheet.Column(7).Width = 10;

                    //Add a title and timestamp at the top of the report
                    workSheet.Cells[1, 1].Value = "Appointment Report";
                    using (ExcelRange Rng = workSheet.Cells[1, 1, 1, 6])
                    {
                        Rng.Merge = true; //Merge columns start and end range
                        Rng.Style.Font.Bold = true; //Font should be bold
                        Rng.Style.Font.Size = 18;
                        Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }
                    //Since the time zone where the server is running can be different, adjust to 
                    //Local for us.
                    DateTime utcDate = DateTime.UtcNow;
                    TimeZoneInfo esTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                    DateTime localDate = TimeZoneInfo.ConvertTimeFromUtc(utcDate, esTimeZone);
                    using (ExcelRange Rng = workSheet.Cells[2, 6])
                    {
                        Rng.Value = "Created: " + localDate.ToShortTimeString() + " on " +
                            localDate.ToShortDateString();
                        Rng.Style.Font.Bold = true; //Font should be bold
                        Rng.Style.Font.Size = 12;
                        Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    }

                    //Ok, time to download the Excel

                    try
                    {
                        Byte[] theData = excel.GetAsByteArray();
                        string filename = "Achivements.xlsx";
                        string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        return File(theData, mimeType, filename);
                    }
                    catch (Exception)
                    {
                        return BadRequest("Could not build and download the file.");
                    }
                }
            }
            return NotFound("No data.");
        }

        private SelectList TroopSelectList(int? selectedId)
        {
            return new SelectList(_context.Troops
                .OrderBy(d => d.TroopName), "ID", "TroopName", selectedId);
        }

        private void PopulateDropDownLists(Scout scout = null)
        {
            ViewData["TroopID"] = TroopSelectList(scout?.TroopID);
        }

        private async Task AddPicture(Scout scout, IFormFile thePicture)
        {
            //Get the picture and save it with the Patient (2 sizes)
            if (thePicture != null)
            {
                string mimeType = thePicture.ContentType;
                long fileLength = thePicture.Length;
                if (!(mimeType == "" || fileLength == 0))//Looks like we have a file!!!
                {
                    if (mimeType.Contains("image"))
                    {
                        using var memoryStream = new MemoryStream();
                        await thePicture.CopyToAsync(memoryStream);
                        var pictureArray = memoryStream.ToArray();//Gives us the Byte[]

                        //Check if we are replacing or creating new
                        if (scout.ScoutPhoto != null)
                        {
                            //We already have pictures so just replace the Byte[]
                            scout.ScoutPhoto.Content = ResizeImage.shrinkImageWebp(pictureArray, 500, 600);

                            //Get the Thumbnail so we can update it.  Remember we didn't include it
                            scout.ScoutThumbnail = _context.scoutThumbnails.Where(p => p.ScoutID == scout.ID).FirstOrDefault();
                            scout.ScoutThumbnail.Content = ResizeImage.shrinkImageWebp(pictureArray, 100, 120);
                        }
                        else //No pictures saved so start new
                        {
                            scout.ScoutPhoto = new ScoutPhoto
                            {
                                Content = ResizeImage.shrinkImageWebp(pictureArray, 500, 600),
                                MimeType = "image/webp"
                            };
                            scout.ScoutThumbnail = new ScoutThumbnail
                            {
                                Content = ResizeImage.shrinkImageWebp(pictureArray, 100, 120),
                                MimeType = "image/webp"
                            };
                        }
                    }
                }
            }
        }


        private bool ScoutExists(int id)
        {
          return _context.Scouts.Any(e => e.ID == id);
        }
    }
}
