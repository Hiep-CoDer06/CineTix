using CineTix.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CineTix.Data;

public class CineTixDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
{
    public CineTixDbContext(DbContextOptions<CineTixDbContext> options)
        : base(options)
    {
    }

    public DbSet<Movie> Movies { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Cinema> Cinemas { get; set; }
    public DbSet<Showtime> Showtimes { get; set; }
    public DbSet<Seat> Seats { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<BookingSeat> BookingSeats { get; set; }
    public DbSet<FoodCombo> FoodCombos { get; set; }
    public DbSet<BookingCombo> BookingCombos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Seat>()
            .HasIndex(seat => new { seat.ShowtimeId, seat.SeatCode })
            .IsUnique();

        modelBuilder.Entity<BookingSeat>()
            .HasIndex(bookingSeat => new
            {
                bookingSeat.BookingId,
                bookingSeat.SeatId
            })
            .IsUnique();

        modelBuilder.Entity<BookingSeat>()
            .HasOne(bookingSeat => bookingSeat.Seat)
            .WithMany(seat => seat.BookingSeats)
            .HasForeignKey(bookingSeat => bookingSeat.SeatId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<BookingCombo>()
            .HasIndex(bookingCombo => new { bookingCombo.BookingId, bookingCombo.FoodComboId })
            .IsUnique();

        modelBuilder.Entity<BookingCombo>()
            .HasOne(bookingCombo => bookingCombo.FoodCombo)
            .WithMany(foodCombo => foodCombo.BookingCombos)
            .HasForeignKey(bookingCombo => bookingCombo.FoodComboId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Booking>()
            .HasOne(booking => booking.User)
            .WithMany()
            .HasForeignKey(booking => booking.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
