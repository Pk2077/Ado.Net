using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class storedProcedures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
                                migrationBuilder.Sql(@"
                            -- Create/Delete procedures
                            CREATE PROCEDURE [dbo].[DeleteStudent]
                                @StudentId int
                            AS
                            BEGIN
                                SET NOCOUNT ON;
                                DELETE FROM [Students] WHERE [Id] = @StudentId;
                            END
                        ");

                                migrationBuilder.Sql(@"
                            CREATE PROCEDURE [dbo].[DeleteStudentCourse]
                                @CourseId int,
                                @StudentId int
                            AS
                            BEGIN
                                SET NOCOUNT ON;
                                DELETE FROM [StudentCourses] WHERE [CoursesId] = @CourseId AND [StudentsId] = @StudentId;
                            END
                        ");

                                migrationBuilder.Sql(@"
                            CREATE PROCEDURE [dbo].[GetCourseById]
                                @CourseId int
                            AS
                            BEGIN
                                SET NOCOUNT ON;
                                SELECT TOP(1) [Id], [Name] FROM [Courses] WHERE [Id] = @CourseId;
                            END
                        ");

                                migrationBuilder.Sql(@"
                            CREATE PROCEDURE [dbo].[GetCourses]
                            AS
                            BEGIN
                                SET NOCOUNT ON;
                                SELECT [Id], [Name] FROM [Courses];
                            END
                        ");

                                migrationBuilder.Sql(@"
                            CREATE PROCEDURE [dbo].[GetDepartments]
                            AS
                            BEGIN
                                SELECT [Id], [Name] FROM [Departments];
                            END
                        ");

                                migrationBuilder.Sql(@"
                            CREATE PROCEDURE [dbo].[GetStudentCourses]
                            AS
                            BEGIN
                                SET NOCOUNT ON;
                                SELECT sc.CoursesId, sc.StudentsId, c.Name AS CourseName, s.Name AS StudentName
                                FROM StudentCourses sc
                                INNER JOIN Courses c ON sc.CoursesId = c.Id
                                INNER JOIN Students s ON sc.StudentsId = s.Id;
                            END
                        ");

                                migrationBuilder.Sql(@"
                            CREATE PROCEDURE [dbo].[GetStudentById]
                                @StudentId int
                            AS
                            BEGIN
                                SET NOCOUNT ON;
                                SELECT [s].[Id] AS [StudentId],
                                       [s].[DepartmentId],
                                       [s].[Name] AS [StudentName],
                                       [d].[Id] AS [DepartmentId],
                                       [d].[Name] AS [DepartmentName],
                                       [c].[StudentsId],
                                       [c].[CourseId],
                                       [c].[CourseName]
                                FROM (
                                    SELECT TOP(1) [s].[Id], [s].[DepartmentId], [s].[Name]
                                    FROM [Students] AS [s]
                                    WHERE [s].[Id] = @StudentId
                                ) AS [s]
                                INNER JOIN [Departments] AS [d] ON [s].[DepartmentId] = [d].[Id]
                                LEFT JOIN (
                                    SELECT [s0].[StudentsId], [c].[Id] AS [CourseId], [c].[Name] AS [CourseName]
                                    FROM [StudentCourses] AS [s0]
                                    INNER JOIN [Courses] AS [c] ON [s0].[CoursesId] = [c].[Id]
                                ) AS [c] ON [s].[Id] = [c].[StudentsId]
                                ORDER BY [s].[Id], [d].[Id], [c].[CourseId], [c].[StudentsId];
                            END
                        ");

                                migrationBuilder.Sql(@"
                            CREATE PROCEDURE [dbo].[UpdateStudentWithCourses]
                                @StudentId int,
                                @DepartmentId int,
                                @Name nvarchar(4000),
                                @CourseIds nvarchar(MAX)
                            AS
                            BEGIN
                                SET NOCOUNT ON;

                                -- Update the student's information
                                UPDATE [Students] SET [DepartmentId] = @DepartmentId, [Name] = @Name WHERE [Id] = @StudentId;

                                -- Delete existing course associations for the student
                                DELETE FROM [StudentCourses] WHERE [StudentsId] = @StudentId;

                                -- Insert the student's courses into the StudentCourses table
                                IF @CourseIds IS NOT NULL AND LEN(@CourseIds) > 0
                                BEGIN
                                    INSERT INTO [StudentCourses] ([CoursesId], [StudentsId])
                                    SELECT value, @StudentId FROM STRING_SPLIT(@CourseIds, ',');
                                END
                            END
                        ");

                                migrationBuilder.Sql(@"
                            CREATE PROCEDURE [dbo].[InsertStudentWithCourses]
                                @DepartmentId int,
                                @Name nvarchar(4000),
                                @CourseIds nvarchar(MAX)
                            AS
                            BEGIN
                                SET NOCOUNT ON;

                                DECLARE @NewStudentId int;

                                -- Insert the student into the Students table and get the generated ID
                                INSERT INTO [Students] ([DepartmentId], [Name]) VALUES (@DepartmentId, @Name);
                                SET @NewStudentId = SCOPE_IDENTITY();

                                -- Insert the student's courses into the StudentCourses table
                                IF @CourseIds IS NOT NULL AND LEN(@CourseIds) > 0
                                BEGIN
                                    INSERT INTO [StudentCourses] ([CoursesId], [StudentsId])
                                    SELECT value, @NewStudentId FROM STRING_SPLIT(@CourseIds, ',');
                                END
                            END
                        ");

                                migrationBuilder.Sql(@"
                            CREATE PROCEDURE [dbo].[GetStudentData]
                            AS
                            BEGIN
                                WITH StudentCourseAggregated AS (
                                    SELECT
                                        [s0].[StudentsId],
                                        [CourseIds] = STRING_AGG(CONVERT(VARCHAR(10), [s0].[CoursesId]), ','),
                                        [CourseNames] = STRING_AGG([c].[Name], ', ')
                                    FROM
                                        [StudentCourses] AS [s0]
                                    INNER JOIN
                                        [Courses] AS [c] ON [s0].[CoursesId] = [c].[Id]
                                    GROUP BY
                                        [s0].[StudentsId]
                                )
                                SELECT
                                    [s].[Id],
                                    [s].[DepartmentId],
                                    [s].[Name] AS [StudentName],
                                    [d].[Id] AS [DepartmentId],
                                    [d].[Name] AS [DepartmentName],
                                    [sc].[CourseIds],
                                    [sc].[CourseNames]
                                FROM
                                    [Students] AS [s]
                                INNER JOIN
                                    [Departments] AS [d] ON [s].[DepartmentId] = [d].[Id]
                                LEFT JOIN
                                    [StudentCourseAggregated] AS [sc] ON [s].[Id] = [sc].[StudentsId]
                                ORDER BY
                                    [s].[Id], [d].[Id];
                            END
                        ");
            migrationBuilder.Sql(@"

                                set Identity_Insert Courses on
                                insert into Courses(Id,Name) values(1,'C#')
                                insert into Courses(Id,Name) values(2,'Asp.Net')
                                insert into Courses(Id,Name) values(3,'Angular')
                                set Identity_Insert Courses off

                                set Identity_Insert Departments on
                                insert into Departments(Id,Name) values(1,'CSE')
                                insert into Departments(Id,Name) values(2,'MECH')
                                insert into Departments(Id,Name) values(3,'CIVIL')
                                set Identity_Insert Departments off
                                ");


        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
