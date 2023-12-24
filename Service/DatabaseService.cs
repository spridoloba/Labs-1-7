using DSP.Models;
using System;
using MySql.Data.MySqlClient;   
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Globalization;
using System.Data;

namespace DSP.Service
{
    public class DatabaseService
    {
        private readonly string _connectionString;
        private readonly ILogger<DatabaseService> _logger;

        public DatabaseService(string host, string database, string username, string password, ILogger<DatabaseService> logger)
        {
            
            _connectionString = $"Server={host};Port=3306;Database={database};User Id={username};Password={password};";
            _logger = logger;
        }

        public async Task<bool> IsTeaserOwnedByParticipant(int teaserId, string participantLogin)
        {
            var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new MySqlCommand("SELECT owner FROM new_teasers WHERE id = @TeaserId", connection);
            command.Parameters.AddWithValue("@TeaserId", teaserId);
           

            var ownerLogin = (string)await command.ExecuteScalarAsync();
            await connection.CloseAsync();

            return ownerLogin == participantLogin;
        }
        public async Task<string> GetCategoryFromIdAsync(int categoryId)
        {
            var iabFilePath = "iab.txt"; 
            var lines = await File.ReadAllLinesAsync(iabFilePath);
            foreach (var line in lines)
            {
                var parts = line.Split(',');
                if (parts.Length == 2 && int.TryParse(parts[0], out int id) && id == categoryId)
                {
                    return parts[1]; 
                }
            }
            return null; 
        }
        public async Task<long>  InsertOfferRecordAsync(OfferRecord record)
        {

            string payoutString = record.Payout.Replace(",", ".");
            if (!decimal.TryParse(payoutString, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal payoutValue))
            {
                throw new InvalidOperationException("Invalid payout value.");
            }

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                long id = GenerateRandomId();
                bool isUnique = false;

                while (!isUnique)
                {
                    var checkIdCommand = new MySqlCommand("SELECT COUNT(*) FROM new_teasers WHERE id = @ID", connection);
                    checkIdCommand.Parameters.AddWithValue("@ID", id);
                    long count = (long)await checkIdCommand.ExecuteScalarAsync();

                    if (count == 0)
                    {
                        isUnique = true;
                    }
                    else
                    {
                        id = GenerateRandomId(); 
                    }
                }



                var command = new MySqlCommand(@"
                INSERT INTO new_teasers 
                (id, peerclickSourceId, country, on_off, scheduleEnabled, schedule, mode, roi_for_sources, roi_for_users, turbo, no_leads, c_limit, start_mcpm, spend_limit_daily, lead_limit_daily, cat, adult, ad_format, n2s, retarget_mode, lang, lang_filter, os_filter, region_filter, city_filter, ad_type, landing_type, dep, team, offer, payout, approve, button_text, pp, crid, owner, mgid_id, link, title, description, image, brand, show_anim, tags, abtest_on, pp_2, crid_2, owner_2, mgid_id_2	, link_2, title_2, description_2, image_2, brand_2, show_anim_2, tags_2, stas_done, comment, metaData) 
                VALUES 
                (@ID, @PeerclickSourceId, @Country, @OnOff, @ScheduleEnabled, @Schedule, @Mode, @RoiForSources, @RoiForUsers, @Turbo, @NoLeads, @CLimit, @StartMcpm, @SpendLimitDaily, @LeadLimitDaily, @Cat, @Adult, @AdFormat, @N2S, @RetargetMode, @Lang, @LangFilter, @OsFilter, @RegionFilter, @CityFilter, @AdType, @LandingType, @Dep, @Team, @Offer, @Payout, @Approve, @ButtonText, @Pp, @Crid, @Owner, @MgidId, @Link, @Title, @Description, @Image, @Brand, @ShowAnim, @Tags, @AbtestOn, @Pp2, @Crid2, @Owner2, @MgidId2, @Link2, @Title2, @Description2, @Image2, @Brand2, @ShowAnim2, @Tags2, @StasDone, @Comment, @MetaData)", connection);

                command.Parameters.AddWithValue("@ID", id);
                command.Parameters.AddWithValue("@PeerclickSourceId", record.PeerclickSourceId);
                command.Parameters.AddWithValue("@Country", record.Country ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@OnOff", record.OnOff);
                command.Parameters.AddWithValue("@ScheduleEnabled", record.ScheduleEnabled);
                command.Parameters.AddWithValue("@Schedule", record.Schedule);
                command.Parameters.AddWithValue("@Mode", record.Mode);
                command.Parameters.AddWithValue("@RoiForSources", record.RoiForSources ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@RoiForUsers", record.RoiForUsers ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Turbo", record.Turbo);
                command.Parameters.AddWithValue("@NoLeads", record.NoLeads);
                command.Parameters.AddWithValue("@CLimit", record.CLimit);
                command.Parameters.AddWithValue("@StartMcpm", record.StartMcpm);
                command.Parameters.AddWithValue("@SpendLimitDaily", record.SpendLimitDaily);
                command.Parameters.AddWithValue("@LeadLimitDaily", record.LeadLimitDaily);
                command.Parameters.AddWithValue("@Cat", record.Cat ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Adult", record.Adult);
                command.Parameters.AddWithValue("@AdFormat", record.AdFormat);
                command.Parameters.AddWithValue("@N2S", record.N2S);
                command.Parameters.AddWithValue("@RetargetMode", record.RetargetMode);
                command.Parameters.AddWithValue("@Lang", record.Lang ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@LangFilter", record.LangFilter ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@OsFilter", record.OsFilter ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@RegionFilter", record.RegionFilter ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@CityFilter", record.CityFilter ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@AdType", record.AdType ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@LandingType", record.LandingType);
                command.Parameters.AddWithValue("@Dep", record.Dep);
                command.Parameters.AddWithValue("@Team", record.Team);
                command.Parameters.AddWithValue("@Offer", record.Offer ?? (object)DBNull.Value);
             
                command.Parameters.AddWithValue("@Payout", record.Payout);
                command.Parameters.AddWithValue("@Approve", record.Approve);
                command.Parameters.AddWithValue("@ButtonText", record.ButtonText ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Pp", record.Pp);
                command.Parameters.AddWithValue("@Crid", record.Crid ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Owner", record.Owner ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@MgidId", record.MgidId);
                command.Parameters.AddWithValue("@Link", record.Link ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Title", record.Title ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Description", record.Description ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Image", record.Image ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Brand", record.Brand ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@ShowAnim", record.ShowAnim);
                command.Parameters.AddWithValue("@Tags", record.Tags ?? string.Empty);
                command.Parameters.AddWithValue("@AbtestOn", record.AbtestOn);
                command.Parameters.AddWithValue("@Pp2", record.Pp2 ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Crid2", record.Crid2 ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Owner2", record.Owner2 ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@MgidId2", record.MgidId2);
                command.Parameters.AddWithValue("@Link2", record.Link2 ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Title2", record.Title2 ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Description2", record.Description2 ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Image2", record.Image2 ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Brand2", record.Brand2 ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@ShowAnim2", record.ShowAnim2);
                command.Parameters.AddWithValue("@Tags2", record.Tags2 ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@StasDone", record.StasDone);
                command.Parameters.AddWithValue("@Comment", record.Comment ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@MetaData", record.MetaData);

                await command.ExecuteNonQueryAsync();

                _logger.LogInformation($"Запис с ID {id} був успішно створений на сервері.");
                return id;
            }
            
        }

        private long GenerateRandomId()
        {
            var random = new Random();
            return random.Next(100000000, 1000000000); 
        }

        public async Task InsertTaskEnableAsync(int offerId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var getOfferCommand = new MySqlCommand("SELECT offer FROM new_teasers WHERE id = @OfferId", connection);
                getOfferCommand.Parameters.AddWithValue("@OfferId", offerId);
                var offer = (string)(await getOfferCommand.ExecuteScalarAsync() ?? string.Empty);

                var maxIdCommand = new MySqlCommand("SELECT MAX(operation_id) FROM dsp_tasks", connection);
                var maxIdResult = await maxIdCommand.ExecuteScalarAsync();
                int nextOperationId = maxIdResult != DBNull.Value ? Convert.ToInt32(maxIdResult) + 1 : 1;

               
                var insertCommand = new MySqlCommand(@"
            INSERT INTO dsp_tasks (
                operation_id, offer_id, offer, on_off, stas_done
            ) VALUES (
                @OperationId, @OfferId, @Offer, 1, 0
            )", connection);

                insertCommand.Parameters.AddWithValue("@OperationId", nextOperationId);
                insertCommand.Parameters.AddWithValue("@OfferId", offerId);
                insertCommand.Parameters.AddWithValue("@Offer", offer);

                await insertCommand.ExecuteNonQueryAsync();
            }
        }

        public async Task InsertTaskDisableAsync(int offerId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var getOfferCommand = new MySqlCommand("SELECT offer FROM new_teasers WHERE id = @OfferId", connection);
                getOfferCommand.Parameters.AddWithValue("@OfferId", offerId);
                var offer = (string)(await getOfferCommand.ExecuteScalarAsync() ?? string.Empty);

                var maxIdCommand = new MySqlCommand("SELECT MAX(operation_id) FROM dsp_tasks", connection);
                var maxIdResult = await maxIdCommand.ExecuteScalarAsync();
                int nextOperationId = maxIdResult != DBNull.Value ? Convert.ToInt32(maxIdResult) + 1 : 1;

                
                var insertCommand = new MySqlCommand(@"
            INSERT INTO dsp_tasks (
                operation_id, offer_id, offer, on_off, stas_done
            ) VALUES (
                @OperationId, @OfferId, @Offer, 0, 0
            )", connection);

                insertCommand.Parameters.AddWithValue("@OperationId", nextOperationId);
                insertCommand.Parameters.AddWithValue("@OfferId", offerId);
                insertCommand.Parameters.AddWithValue("@Offer", offer);

                await insertCommand.ExecuteNonQueryAsync();
            }
        }

        public async Task InsertTaskRoiAsync(int offerId, int roiSource, int roiUsers)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var getOfferCommand = new MySqlCommand("SELECT offer FROM new_teasers WHERE id = @OfferId", connection);
                getOfferCommand.Parameters.AddWithValue("@OfferId", offerId);
                var offer = (string)(await getOfferCommand.ExecuteScalarAsync() ?? string.Empty);

                var maxIdCommand = new MySqlCommand("SELECT MAX(operation_id) FROM dsp_tasks", connection);
                var maxIdResult = await maxIdCommand.ExecuteScalarAsync();
                int nextOperationId = maxIdResult != DBNull.Value ? Convert.ToInt32(maxIdResult) + 1 : 1;

                
                var insertCommand = new MySqlCommand(@"
            INSERT INTO dsp_tasks (
                operation_id, offer_id, offer, roi_for_sources, roi_for_users, stas_done
            ) VALUES (
                @OperationId, @OfferId, @Offer, @Roiforsources, @Roiforusers, 0
            )", connection);

                insertCommand.Parameters.AddWithValue("@OperationId", nextOperationId);
                insertCommand.Parameters.AddWithValue("@OfferId", offerId);
                insertCommand.Parameters.AddWithValue("@Offer", offer);
                insertCommand.Parameters.AddWithValue("@Roiforsources", roiSource);
                insertCommand.Parameters.AddWithValue("@Roiforusers", roiUsers);

                await insertCommand.ExecuteNonQueryAsync();
            }
        }

        public async Task InsertTaskModeAsync(int offerId, int mode)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var getOfferCommand = new MySqlCommand("SELECT offer FROM new_teasers WHERE id = @OfferId", connection);
                getOfferCommand.Parameters.AddWithValue("@OfferId", offerId);
                var offer = (string)(await getOfferCommand.ExecuteScalarAsync() ?? string.Empty);

                var maxIdCommand = new MySqlCommand("SELECT MAX(operation_id) FROM dsp_tasks", connection);
                var maxIdResult = await maxIdCommand.ExecuteScalarAsync();
                int nextOperationId = maxIdResult != DBNull.Value ? Convert.ToInt32(maxIdResult) + 1 : 1;

                
                var insertCommand = new MySqlCommand(@"
            INSERT INTO dsp_tasks (
                operation_id, offer_id, offer, mode, stas_done
            ) VALUES (
                @OperationId, @OfferId, @Offer, @Mode, 0
            )", connection);

                insertCommand.Parameters.AddWithValue("@OperationId", nextOperationId);
                insertCommand.Parameters.AddWithValue("@OfferId", offerId);
                insertCommand.Parameters.AddWithValue("@Offer", offer);
                insertCommand.Parameters.AddWithValue("@Mode", mode);

                await insertCommand.ExecuteNonQueryAsync();
            }
        }


        public async Task InsertTaskPayoutAsync(int offerId, string payoutString)
        {
            

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();


                payoutString = payoutString.Replace(",", ".");

                
                if (!float.TryParse(payoutString, NumberStyles.Any, CultureInfo.InvariantCulture, out float payout))
                {
                    throw new FormatException("Invalid payout format.");
                }


                var getOfferCommand = new MySqlCommand("SELECT offer FROM new_teasers WHERE id = @OfferId", connection);
                getOfferCommand.Parameters.AddWithValue("@OfferId", offerId);
                var offer = (string)(await getOfferCommand.ExecuteScalarAsync() ?? string.Empty);

                var maxIdCommand = new MySqlCommand("SELECT MAX(operation_id) FROM dsp_tasks", connection);
                var maxIdResult = await maxIdCommand.ExecuteScalarAsync();
                int nextOperationId = maxIdResult != DBNull.Value ? Convert.ToInt32(maxIdResult) + 1 : 1;

               
                var insertCommand = new MySqlCommand(@"
            INSERT INTO dsp_tasks (
                operation_id, offer_id, offer, payout, stas_done
            ) VALUES (
                @OperationId, @OfferId, @Offer, @Payout, 0
            )", connection);

                insertCommand.Parameters.AddWithValue("@OperationId", nextOperationId);
                insertCommand.Parameters.AddWithValue("@OfferId", offerId);
                insertCommand.Parameters.AddWithValue("@Offer", offer);
                insertCommand.Parameters.AddWithValue("@Payout", payout);

                await insertCommand.ExecuteNonQueryAsync();
            }
        }


        public async Task InsertTaskClimitAsync(int offerId, string climit)
        {


            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();


                climit = climit.Replace(",", ".");

               
                if (!float.TryParse(climit, NumberStyles.Any, CultureInfo.InvariantCulture, out float climitt))
                {
                    throw new FormatException("Invalid climit format.");
                }


                var getOfferCommand = new MySqlCommand("SELECT offer FROM new_teasers WHERE id = @OfferId", connection);
                getOfferCommand.Parameters.AddWithValue("@OfferId", offerId);
                var offer = (string)(await getOfferCommand.ExecuteScalarAsync() ?? string.Empty);

                var maxIdCommand = new MySqlCommand("SELECT MAX(operation_id) FROM dsp_tasks", connection);
                var maxIdResult = await maxIdCommand.ExecuteScalarAsync();
                int nextOperationId = maxIdResult != DBNull.Value ? Convert.ToInt32(maxIdResult) + 1 : 1;

               
                var insertCommand = new MySqlCommand(@"
            INSERT INTO dsp_tasks (
                operation_id, offer_id, offer, c_limit, stas_done
            ) VALUES (
                @OperationId, @OfferId, @Offer, @C_limit, 0
            )", connection);

                insertCommand.Parameters.AddWithValue("@OperationId", nextOperationId);
                insertCommand.Parameters.AddWithValue("@OfferId", offerId);
                insertCommand.Parameters.AddWithValue("@Offer", offer);
                insertCommand.Parameters.AddWithValue("@C_limit", climitt);

                await insertCommand.ExecuteNonQueryAsync();
            }
        }

    }
}
